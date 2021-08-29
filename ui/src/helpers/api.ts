import _axios from "axios";

const axios = _axios.create({
  baseURL:
    process.env.NODE_ENV === "development" ? "http://localhost:5000" : "/",
});

export type TSearchResult = {
  speed: number;
  documents: TSearchResultDocument[];
};

export type TSearchResultDocument = {
  title: string;
  description: string;
  link: string;
};

const api = {
  search: {
    async execute(query: string): Promise<TSearchResult> {
      return axios.get(`/search`, { params: { query } })
        .then(resp => resp.data);
    },
  },
};

export default api;
