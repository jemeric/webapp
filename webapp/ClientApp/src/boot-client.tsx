import "./components/app.scss"; // needed for compilation to style.css
import * as React from "react";
import * as ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import { App } from "./components/App";
import { BrowserRouter } from "react-router-dom";
import { AuthContext, IAuthContextData } from "./auth-context";

function renderApp(authContextData: IAuthContextData) {
  const authContext: AuthContext = new AuthContext(authContextData);
  const Context = React.createContext(authContext);

  // This code starts up the React app when it runs in a browser.
  const newLocal = (
    <Context.Provider value={authContext}>
      <AppContainer>
        <BrowserRouter>
          <App compiler="Typescript" framework="React" />
        </BrowserRouter>
      </AppContainer>
    </Context.Provider>
  );
  const appTag = document.getElementById("appId");
  // only hydrate if there is something to be hydrated (may not be the case in local dev)
  if (appTag!.childNodes.length) {
    ReactDOM.hydrate(newLocal, appTag);
  } else {
    ReactDOM.render(newLocal, appTag);
  }
}

function initializeAppClient(authContextData: IAuthContextData) {
  renderApp(authContextData);

  // Allow Hot Module Replacement
  if (module.hot) {
    module.hot.accept(() => {
      renderApp(authContextData);
    });
  }
}
