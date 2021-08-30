import clsx from "clsx";
import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import Footer from "../components/Footer";
import Search from "../components/Search";
import ClockIcon from "../icons/ClockIcon";

import {
  chooseCompletion,
  findCompletions,
  selectCompletions,
  selectQuery,
  updateQuery,
} from "../store/slices/searchSlice";

export default function Home() {
  const query = useSelector(selectQuery);
  const completions = useSelector(selectCompletions);
  const dispatch = useDispatch();

  const onQueryChange = (event: React.ChangeEvent) => {
    const target = event.target as HTMLInputElement;
    dispatch(updateQuery(target.value));
    if (target.value.trim()) dispatch(findCompletions(target.value));
  };

  const pickSuggestion = (s: string) => {
    dispatch(chooseCompletion(s));
  };

  return (
    <div className="flex flex-col justify-between min-h-screen py-3 px-4 max-w-5xl mx-auto">
      <div className="flex justify-end">
        <Link to="/history" className="text-primary-400 flex items-center">
          <ClockIcon className="mr-1" /> History
        </Link>
      </div>
      <form action="/search" method="GET" role="search" autoComplete="off">
        <div className="-mt-12 text-center space-y-3 flex flex-col items-center">
          <h2 className="text-4xl text-center">
            <span className="text-gray-200 text-3xl">ask</span>{" "}
            <span className="text-primary-400">Axle.</span>
          </h2>
          <div className="w-full max-w-lg relative">
            <Search
              name="q"
              query={query}
              onChange={onQueryChange}
              className={clsx({})}
              required
            />
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
                <button
                  type="submit"
                  className="mb-2 mx-auto block py-2 px-2 w-24 bg-gray-700 text-white rounded-lg border border-transparent hover:border-white hover:shadow-md"
                >
                  Search
                </button>
              </div>
            ) : null}
          </div>
          <button
            type="submit"
            className="py-2 px-2 w-24 bg-gray-700 text-white rounded-lg border border-transparent hover:border-white hover:shadow-md"
          >
            Search
          </button>
        </div>
      </form>
      <div>
        <Footer />
      </div>
    </div>
  );
}
