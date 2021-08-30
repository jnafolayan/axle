import React from "react";
import clsx from "clsx";
import SearchIcon from "../icons/SearchIcon";

type TSearchProps = {
  query: string;
  onChange: (event: React.ChangeEvent) => void;
  name: string;
  className?: string;
  required: boolean;
};

export default function Search(props: TSearchProps) {
  const { name, query, className, onChange, required } = props;
  return (
    <div className="relative w-full text-base md:text-lg">
      <input
        name={name}
        value={query}
        className={clsx(
          "bg-transparent text-gray-200 border-2 border-gray-200 h-10 md:h-12 py-2 pl-4 pr-14 rounded-full w-full outline-none hover:shadow-sm",
          className
        )}
        onChange={onChange}
        required={required}
      />

      <button
        type="submit"
        role="banner"
        className="absolute top-0 right-0 flex justify-center items-center border-2 border-gray-200 text-gray-200 w-10 h-10 md:w-12 md:h-12 rounded-full hover:bg-primary-400 hover:text-gray-900"
      >
        <SearchIcon className="text-xl" />
      </button>
    </div>
  );
}
