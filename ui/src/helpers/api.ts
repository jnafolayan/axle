import _axios from "axios";
import { API_BASE_URL } from "../constants";

const axios = _axios.create({
  baseURL: API_BASE_URL,
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

export type TUploadPayload = {
  type: string;
  title?: string;
  description?: string;
  documents?: File[];
  link?: string;
};
export type TUploadResult = {
  status: string;
  message: string;
  errors: { status: string; message: string }[];
};

export type TAutoCompleteResult = {
  query: string;
  suggestions: string[];
};

const api = {
  search: {
    async execute(query: string): Promise<TSearchResult> {
      return axios
        .get(`/search`, { params: { query } })
        .then((resp) => resp.data);
    },
    async autocomplete(query: string): Promise<TAutoCompleteResult> {
      return axios.get("/autocomplete", { params: { query } })
        .then((resp) => resp.data);
    }
  },
  documents: {
    getDownloadURL(suffix: string) {
      return `${API_BASE_URL}${suffix}`;
    },
    async upload(payload: TUploadPayload): Promise<TUploadResult> {
      const fd = new FormData();
      fd.append("type", payload.type);
      if (payload.title) fd.append("title", payload.title);
      if (payload.description) fd.append("description", payload.description);
      if (payload.link) fd.append("link", payload.link);
      if (payload.documents)
        for (let doc of payload.documents) fd.append("documents", doc);
      return axios.post("/upload", fd).then((resp) => resp.data);
    },
    async absolutePath(payload: string): Promise<TUploadResult> {
      const fd = new FormData();
      fd.append("path", payload);
      return axios
        .post("/uploadfolder", fd)
        .then((resp) => resp.data);
    },
  },
};

export default api;
