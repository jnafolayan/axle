import { MAX_QUERIES_IN_STORE } from "../constants";

export type TQueryItem = {
  query: string;
  timestamp: number;
};

const storage = {
  historyKey: "axle_history",
  getHistory(): TQueryItem[] {
    try {
      const list = localStorage.getItem(storage.historyKey);
      return list ? JSON.parse(list) : [];
    } catch (_error) {
      return [];
    }
  },
  saveQuery(query: string) {
    const list = storage.getHistory();
    list.unshift({ query, timestamp: new Date().getTime() });
    if (list.length > MAX_QUERIES_IN_STORE) list.length = MAX_QUERIES_IN_STORE;
    localStorage.setItem(storage.historyKey, JSON.stringify(list));
  },
  deleteQuery(query: string) {
    const list = storage.getHistory().filter((q) => q.query !== query);
    localStorage.setItem(storage.historyKey, JSON.stringify(list));
  },
  clear() {
    localStorage.setItem(storage.historyKey, "[]");
  },
};

export default storage;
