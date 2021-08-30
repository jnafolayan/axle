import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import ClockIcon from "../icons/ClockIcon";
import {
  chooseCompletion,
  findCompletions,
  resetCompletions,
  selectCompletions,
  selectQuery,
  updateQuery,
} from "../store/slices/searchSlice";
import Search from "./Search";

export default function Header() {
  const query = useSelector(selectQuery);
  const completions = useSelector(selectCompletions);
  const dispatch = useDispatch();

  const onQueryChange = (event: React.ChangeEvent) => {
    const target = event.target as HTMLInputElement;
    dispatch(updateQuery(target.value));
    if (target.value.trim()) dispatch(findCompletions(target.value.trim()));
  };

  const pickSuggestion = (s: string) => {
    dispatch(chooseCompletion(s));
  };
  
  useEffect(() => {
    document.addEventListener("click", () => {
      dispatch(resetCompletions());
    });
  }, []);

  return (
    <div className="flex justify-between items-center">
      <div className="flex items-center justify-between md:space-x-4 w-full md:w-4/5">
        <Link
          to="/"
          className="text-primary-400 whitespace-nowrap hidden md:block md:text-2xl"
        >
          Axle
        </Link>
        <div className="w-full relative">
          <form action="/search" method="GET" autoComplete="off">
            <Search name="q" query={query} onChange={onQueryChange} required />
          </form>
          {completions.suggestions.length ? (
            <div className="shadow-md text-gray-200 text-left cursor-default bg-gray-900 w-full absolute top-full rounded-b-lg">
              {completions.suggestions.map((suggestion, i) => (
                <div
                  key={i + suggestion}
                  className="px-3 py-2 hover:bg-gray-600 truncate"
                  onClick={() => pickSuggestion(suggestion)}
                >
                  {suggestion}
                </div>
              ))}
            </div>
          ) : null}
        </div>
      </div>
      <div className="hidden md:block">
        <Link to="/history" className="text-primary-400 flex items-center">
          <ClockIcon className="mr-1" /> History
        </Link>
      </div>
    </div>
  );
}
