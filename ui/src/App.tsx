import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import HomePage from "./pages/Home";
import SearchViewPage from "./pages/SearchView";
import HistoryPage from "./pages/History";

function App() {
  return (
    <div className="bg-gray-900 min-h-screen">
      <Router>
        <Switch>
          <Route path="/" exact>
            <HomePage />
          </Route>
          <Route path="/search" exact>
            <SearchViewPage />
          </Route>
          <Route path="/history" exact>
            <HistoryPage />
          </Route>
        </Switch>
      </Router>
    </div>
  );
}

export default App;
