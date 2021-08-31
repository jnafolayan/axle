import clsx from "clsx";
import { useEffect, useState } from "react";
import Skeleton, { SkeletonTheme } from "react-loading-skeleton";
import { useDispatch, useSelector } from "react-redux";
import { useHistory, useLocation } from "react-router-dom";
import Container from "../components/Container";
import Header from "../components/Header";
import Main from "../components/Main";
import SearchDocument from "../components/SearchDocument";
import { MAX_PAGES_VISIBLE, MAX_RESULTS_PER_PAGE } from "../constants";
import { TSearchResultDocument } from "../helpers/api";
import storage from "../helpers/storage";
import {
  executeQuery,
  selectError,
  selectLoading,
  selectQuery,
  selectSearchResult,
  updateQuery,
} from "../store/slices/searchSlice";

export default function SearchView() {
  const location = useLocation();
  const history = useHistory();
  const query = useSelector(selectQuery);
  const error = useSelector(selectError);
  const loading = useSelector(selectLoading);
  const searchResult = useSelector(selectSearchResult);
  const dispatch = useDispatch();

  const [currentQuery, setCurrentQuery] = useState("");
  const [pages, setPages] = useState<number[]>([]);
  const [currentPage, setCurrentPage] = useState(-1);
  const [currentDocuments, setCurrentDocuments] = useState<
    TSearchResultDocument[]
  >([]);

  const formatNumber = (num: number) => {
    const numStr = String(num);
    let result = "";
    let c = 0,
      i = numStr.length - 1;
    for (; i >= 0; i--) {
      if (c++ % 3 === 0 && i !== numStr.length - 1) result = ", " + result;
      result = numStr[i] + result;
    }
    return result;
  };

  const formatElapsed = (elapsed: number) => {
    console.log({ elapsed }, elapsed / 1000);
    return (elapsed / 1000).toFixed(2) + " seconds";
  };

  const gotoPage = (page: number, forceChange: boolean = false) => {
    if (page === -1) {
      setCurrentPage(-1);
      setCurrentDocuments([]);
      return;
    }

    if (!searchResult) return;
    if (page < 1 || page > pages.length) return;
    if (page === currentPage && !forceChange) return;

    console.log({ page })

    const start = (page - 1) * MAX_RESULTS_PER_PAGE;
    const end = start + MAX_RESULTS_PER_PAGE;

    setCurrentPage(page);
    setCurrentDocuments(searchResult.documents.slice(start, end));
  };

  const gotoPreviousPage = () => gotoPage(currentPage - 1);
  const gotoNextPage = () => gotoPage(currentPage + 1);

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    let q = params.get("q") || "";
    q = q.trim();
    if (!q) history.push("/");
    else {
      dispatch(updateQuery(q));
      dispatch(executeQuery(q));

      // save query to disk
      storage.saveQuery(q);
    }
  }, [location, history, dispatch]);

  useEffect(() => {
    if (searchResult) {
      const { documents } = searchResult;
      setCurrentQuery(query);

      let numPages = Math.ceil(documents.length / MAX_RESULTS_PER_PAGE);
      numPages = Math.min(numPages, MAX_PAGES_VISIBLE);

      if (numPages > 0) {
        const pages: number[] = new Array(numPages)
          .fill(0)
          .map((_, i) => i + 1);
        setPages(pages);

        const cur = pages[0];
        setCurrentPage(cur);

        const start = (cur - 1) * MAX_RESULTS_PER_PAGE;
        const end = start + MAX_RESULTS_PER_PAGE;
        setCurrentDocuments(searchResult.documents.slice(start, end));
        return;
      }
    }
    setPages([]);
    setCurrentPage(-1);
    setCurrentDocuments([]);
    // eslint-disable-next-line
  }, [searchResult]);

  return (
    <Container>
      <Header />

      <div className="my-4 h-1 bg-gray-800"></div>

      <Main>
        {loading ||
        (!loading && currentPage === -1 && searchResult?.documents.length) ? (
          <Loading />
        ) : error !== "" ? (
          <div>
            <p>{error}</p>
          </div>
        ) : currentDocuments.length === 0 ? (
          <div className="py-12">
            <p>
              Your search -{" "}
              <span className="text-white font-bold">{currentQuery}</span> - did not
              match any documents.
            </p>
            <h5 className="mt-4">Suggestions</h5>
            <ul className="list-disc pl-5">
              <li className="list-item">
                Make sure that all words are spelled correctly.
              </li>
              <li>Try different keywords.</li>
              <li>Try more general keywords.</li>
            </ul>
          </div>
        ) : searchResult && currentDocuments.length > 0 ? (
          <div>
            <p className="text-gray-500 text-sm mb-4">
              About {formatNumber(searchResult.documents.length)} results (
              {formatElapsed(searchResult?.speed || 0)})
            </p>

            <div className="space-y-8">
              {currentDocuments.map((doc, i) => (
                <SearchDocument key={i+doc.link} value={doc} />
              ))}
            </div>

            <div className="flex justify-center space-x-2 mt-8 text-sm">
              <button
                onClick={gotoPreviousPage}
                disabled={currentPage <= 1}
                className={clsx({ "text-gray-500": currentPage <= 1 })}
              >
                Previous
              </button>
              {pages.map((page) => (
                <button
                  key={page}
                  className={clsx("border border-gray-200 w-8 h-8", {
                    "bg-gray-200 text-black": page === currentPage,
                  })}
                  onClick={() => gotoPage(page)}
                >
                  {page}
                </button>
              ))}
              <button
                onClick={gotoNextPage}
                disabled={currentPage >= pages.length}
                className={clsx({
                  "text-gray-500": currentPage >= pages.length,
                })}
              >
                Next
              </button>
            </div>
          </div>
        ) : null}
      </Main>
    </Container>
  );
}

function Loading() {
  const rows = [0, 0, 0, 0, 0];

  const row = (i: number) => (
    <div key={i} className="w-full">
      <Skeleton height={20} delay={i * 0.2} />
      <Skeleton height={60} delay={i * 0.2} />
    </div>
  );

  return (
    <SkeletonTheme color="#202020" highlightColor="#444">
      <div className="space-y-4">{rows.map((_, i) => row(i))}</div>
    </SkeletonTheme>
  );
}
