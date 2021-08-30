export const MAX_RESULTS_PER_PAGE = 8;

export const MAX_PAGES_VISIBLE = 5;

export const MAX_QUERIES_IN_STORE = 50;

export const API_BASE_URL =
  process.env.NODE_ENV === "development" ? "http://localhost:5000" : "";
