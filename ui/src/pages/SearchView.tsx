import { useEffect } from "react";
import Skeleton, { SkeletonTheme } from "react-loading-skeleton";
import { useDispatch, useSelector } from "react-redux";
import { useHistory, useLocation } from "react-router-dom";
import Header from "../components/Header";
import Main from "../components/Main";
import SearchDocument from "../components/SearchDocument";
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

  console.log({ searchResult, loading });

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
    return (elapsed / 1000).toFixed(2) + " seconds";
  };

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const q = params.get("q");
    dispatch(updateQuery(q || ""));
    if (q) {
      dispatch(executeQuery(q));

      // save query to disk
      storage.saveQuery(q);
    } else history.push("/");
  }, [location, history, dispatch]);

  return (
    <div className="max-w-5xl mx-auto py-3 px-4">
      <Header />

      <div className="my-4 h-1 bg-gray-800"></div>

      <Main>
        {loading ? (
          <Loading />
        ) : error !== "" ? (
          <div>
            <p>{error}</p>
          </div>
        ) : searchResult && searchResult.documents.length === 0 ? (
          <div className="py-12">
            <p>
              Your search -{" "}
              <span className="text-white font-bold">{query}</span> - did not
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
        ) : searchResult && searchResult.documents.length > 0 ? (
          <div>
            <p>
              About {formatNumber(searchResult.documents.length)} results (
              {formatElapsed(searchResult.speed)})
            </p>

            <div className="space-y-2">
              {searchResult.documents.map((doc) => (
                <SearchDocument key={doc.link} value={doc} />
              ))}
            </div>
          </div>
        ) : null}
      </Main>
    </div>
  );
}

function Loading() {
  const rows = [0, 0, 0, 0, 0];

  const row = (i: number) => (
    <div className="w-full">
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
