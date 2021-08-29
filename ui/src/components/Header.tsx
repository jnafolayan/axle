import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import ClockIcon from "../icons/ClockIcon";
import { selectQuery, updateQuery } from "../store/slices/searchSlice";
import Search from "./Search";

export default function Header() {
  const query = useSelector(selectQuery);
  const dispatch = useDispatch();
  
  const onQueryChange = (event: React.ChangeEvent) => {
    const target = event.target as HTMLInputElement;
    dispatch(updateQuery(target.value));
  };
  
  return (
    <div className="flex justify-between items-center">
      <div className="flex items-center justify-between md:space-x-4 w-full md:w-4/5">
        <Link
          to="/"
          className="text-primary-400 whitespace-nowrap hidden md:block md:text-2xl"
        >
          <span className="text-gray-200 text-base md:text-xl">ask</span> Axle
        </Link>
        <form
          action="/search"
          method="GET"
          className="w-full"
          autoComplete="off"
        >
          <Search
            name="q"
            query={query}
            className="h-10 md:h-12"
            onChange={onQueryChange}
            required
          />
        </form>
      </div>
      <div className="hidden md:block">
        <Link to="/history" className="text-primary-400 flex items-center">
          <ClockIcon className="mr-1" /> History
        </Link>
      </div>
    </div>
  );
}
