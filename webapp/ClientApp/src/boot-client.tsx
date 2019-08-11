import * as React from "react";
import * as ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import { App } from "./components/App";
import { BrowserRouter } from "react-router-dom";
import {
  Provider,
  AuthContext,
  IAuthContextData,
  UserRole
} from "./auth-context";

function renderApp(authContextData: IAuthContextData) {
  const authContext: AuthContext = new AuthContext(authContextData);

  // This code starts up the React app when it runs in a browser.
  const clientApp = (
    <AppContainer>
      <Provider value={authContext}>
        <BrowserRouter>
          <App compiler="Typescript" framework="React" />
        </BrowserRouter>
      </Provider>
    </AppContainer>
  );
  const appTag = document.getElementById("appId");
  // only hydrate if there is something to be hydrated (may not be the case in local dev)
  if (appTag!.childNodes.length) {
    ReactDOM.hydrate(clientApp, appTag);
  } else {
    ReactDOM.render(clientApp, appTag);
  }
}

// must cast as any to set property on window
const _global = (window /* browser */ || global) /* node */ as any;
renderApp(_global.authorizationContext); // will be called on hot-reload (should be a nicer way to do this)

// Allow Hot Module Replacement
if (module.hot) {
  module.hot.accept(); // seems to be a disconnect if you try to load on specific module
}