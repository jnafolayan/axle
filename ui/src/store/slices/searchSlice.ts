import {
  createAction,
  createAsyncThunk,
  createDraftSafeSelector,
  createSlice,
  PayloadAction,
} from "@reduxjs/toolkit";
import api, { TAutoCompleteResult, TSearchResult } from "../../helpers/api";

// TYPES
type TSearchState = {
  query: string;
  result: TSearchResult | null;
  error: string;
  loading: boolean;
  completions: { query: string; suggestions: string[] };
};

type TState = {
  [key: string]: any;
  search: TSearchState;
};

// selectors
const selectSelf = (state: TState) => state.search;
export const selectQuery = createDraftSafeSelector(
  selectSelf,
  (state: TSearchState) => state.query
);
export const selectError = createDraftSafeSelector(
  selectSelf,
  (state: TSearchState) => state.error
);
export const selectLoading = createDraftSafeSelector(
  selectSelf,
  (state: TSearchState) => state.loading
);
export const selectSearchResult = createDraftSafeSelector(
  selectSelf,
  (state: TSearchState) => state.result
);
export const selectCompletions = createDraftSafeSelector(
  selectSelf,
  (state: TSearchState) => state.completions
);

// actions
export const resetSearch = createAction("search/reset");
export const updateQuery = createAction<string>("search/updateQuery");
export const setLoading = createAction<boolean>("search/setLoading");
export const chooseCompletion = createAction<string>("search/chooseCompletion");
export const resetCompletions = createAction("search/resetCompletions");
export const executeQuery = createAsyncThunk(
  "search/executeQuery",
  async (query: string, thunkApi) => {
    thunkApi.dispatch(setLoading(true));
    try {
      const results = await api.search.execute(query.trim());
      return results;
    } catch (error: any) {
      console.log({ error });
      return thunkApi.rejectWithValue(error.response);
    } finally {
      thunkApi.dispatch(setLoading(false));
    }
  }
);
export const findCompletions = createAsyncThunk(
  "search/findCompletions",
  async (query: string, thunkApi) => {
    try {
      const results = await api.search.autocomplete(query.trim());
      return results;
    } catch (error: any) {
      console.log({ error });
      return thunkApi.rejectWithValue(error.response);
    }
  }
);

export const searchSlice = createSlice({
  name: "search",
  initialState: {
    query: "",
    result: null,
    error: "",
    loading: false,
    completions: { query: "", suggestions: [] },
  } as TSearchState,
  reducers: {},
  extraReducers: {
    [resetSearch.type]: (state) => {
      state.query = "";
      state.error = "";
      state.result = null;
      state.loading = false;
      state.completions = { query: "", suggestions: [] };
    },
    [updateQuery.type]: (state, action: PayloadAction<string>) => {
      state.query = action.payload;
    },
    [setLoading.type]: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    [chooseCompletion.type]: (state, action: PayloadAction<string>) => {
      state.completions = { query: "", suggestions: [] };
      state.query = action.payload;
    },
    [resetCompletions.type]: (state, action: PayloadAction) => {
      state.completions = { query: "", suggestions: [] };
    },
    [executeQuery.fulfilled.toString()]: (
      state,
      action: PayloadAction<TSearchResult>
    ) => {
      state.result = action.payload;
    },
    [executeQuery.rejected.toString()]: (state) => {
      state.result = null;
      state.error = "An error occured while fetching your results.";
    },
    [findCompletions.fulfilled.toString()]: (
      state,
      action: PayloadAction<TAutoCompleteResult>
    ) => {
      state.completions = {
        query: action.payload.query,
        suggestions: Array.from(new Set(action.payload.suggestions))
          .map((s) => s.substring(0, 80))
          .sort((a, b) => a.length - b.length),
      };
    },
    [findCompletions.rejected.toString()]: (state) => {
      state.completions = { query: "", suggestions: [] };
    },
  },
});

export default searchSlice.reducer;
