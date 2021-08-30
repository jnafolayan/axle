import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Header from "../components/Header";
import Main from "../components/Main";
import storage, { TQueryItem } from "../helpers/storage";

type TDateGroup = {
  date: string;
  list: { q: TQueryItem; time: string }[];
};

export default function History() {
  const [dateGroups, setDateGroups] = useState<TDateGroup[]>([]);

  const clearHistory = () => {
    storage.clear();
    window.location.reload();
  };

  useEffect(() => {
    const map: { [key: string]: { q: TQueryItem; time: string }[] } = {};
    const list = storage.getHistory();
    const result: TDateGroup[] = [];
    list.forEach((q) => {
      const d = new Date(q.timestamp);
      const date = d.toDateString();
      const [left, timeOfDay] = d.toLocaleTimeString().split(" ");
      const [h, m] = left.split(":");
      const time = ([h, m].join(":") + " " + (timeOfDay || "")).trim();

      if (!(date in map)) {
        map[date] = [];
        result.push({
          date,
          list: map[date],
        });
      }

      map[date].push({ q, time });
    });
    setDateGroups(result);
  }, []);

  return (
    <div className="max-w-5xl mx-auto py-3 px-4">
      <Header />

      <div className="my-4 h-1 bg-gray-800"></div>

      <Main>
        {dateGroups.length === 0 ? (
          <div>
            <p>You have not made any searches yet.</p>
          </div>
        ) : (
          <div>
            <button
              className="block ml-auto mb-3 text-base text-right text-primary-400"
              onClick={clearHistory}
            >
              Clear
            </button>
            <div className="space-y-6">
              {dateGroups.map((group) => (
                <div
                  key={group.date}
                  className="border border-gray-800 rounded-lg shadow-lg"
                >
                  <h4 className="border-b border-gray-800 px-3 py-3 font-medium bg-primary-600 rounded-t-lg">
                    {group.date}
                  </h4>
                  <div className="space-y-1 py-2">
                    {group.list.map(({ q, time }) => (
                      <div
                        key={q.timestamp}
                        className="flex space-x-4 items-center px-3 py-2 text-base"
                      >
                        <span className="text-gray-500">{time}</span>
                        <Link to={"/search?q=" + q.query} className="">
                          {q.query}
                        </Link>
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </Main>
    </div>
  );
}
