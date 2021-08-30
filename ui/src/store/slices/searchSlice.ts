import {
  createAction,
  createAsyncThunk,
  createDraftSafeSelector,
  createSlice,
  PayloadAction,
} from "@reduxjs/toolkit";
import api, { TSearchResult } from "../../helpers/api";

// TYPES
type TSearchState = {
  query: string;
  result: TSearchResult | null;
  error: string;
  loading: boolean;
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

// actions
export const resetSearch = createAction("search/reset");
export const updateQuery = createAction<string>("search/updateQuery");
export const setLoading = createAction<boolean>("search/setLoading");
export const executeQuery = createAsyncThunk(
  "search/executeQuery",
  async (query: string, thunkApi) => {
    thunkApi.dispatch(setLoading(true));
    try {
      const results = await api.search.execute(query);
      return results;
    } catch (error) {
      console.log({error});
      return thunkApi.rejectWithValue(error.response);
    } finally {
      thunkApi.dispatch(setLoading(false));
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
  } as TSearchState,
  reducers: {},
  extraReducers: {
    [resetSearch.type]: (state) => {
      state.query = "";
      state.error = "";
      state.result = null;
      state.loading = false;
    },
    [updateQuery.type]: (state, action: PayloadAction<string>) => {
      state.query = action.payload;
    },
    [setLoading.type]: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
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
  },
});

export default searchSlice.reducer;
