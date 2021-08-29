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
    localStorage.setItem(storage.historyKey, JSON.stringify(list));
  },
  deleteQuery(query: string) {
    const list = storage.getHistory().filter((q) => q.query !== query);
    localStorage.setItem(storage.historyKey, JSON.stringify(list));
  },
};

export default storage;
