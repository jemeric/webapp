﻿import * as React from "react";
import { createServerRenderer, RenderResult } from "aspnet-prerendering";
import { renderToString } from "react-dom/server";
import { StaticRouter } from "react-router-dom";
import { App } from "./components/App";
import { ServerStyleSheet } from "styled-components";

// todo isolate and cache server-side template for easier development
function getTemplate(app: string, styles: string, data: IAppData): string {
  return `<head>
                <meta charset="UTF-8" />
                <title>Hello React!</title>
                <link rel="stylesheet" type="text/css" href="/dist/${data.assetsVersion}/style.css">
                ${styles}
            </head>
            <body>
                <div id="appId">${app}</div>
                ${data.externalScripts.map(url => `<script src="${url}"></script>`).join("")}
                <!-- Main -->
                <script src="/dist/${data.assetsVersion}/main-client.js"></script>
            </body>`;
}

interface IAppData {
  externalScripts: string[];
  assetsVersion: string;
  authContextData: IAuthContextData;
}

interface IAuthContextData {
  authenticated: boolean; // to check if authenticated or not
  user: IUserContext; // store all the user details
  accessToken: string | null; // accessToken of user for authorization (To-Do - change with cookie auth?)  
}

interface IAuthContext extends IAuthContextData {
  initiateLogin: () => {}; // start the login process
  handleAuthentication: () => {}; // handle login process
  logout: () => {}; // logout the user
}

interface IUserContext {
  id: string; // data.sub
  email: string | null;
  role: "ADMIN" | "USER" | "GUEST";
}

export default createServerRenderer(params => {
  return new Promise<RenderResult>((resolve, reject) => {
    // this context object contains the results of the render (i.e. context.url will contain URL to redirect to if <Redirect> was used)
    const routerContext: any = {};
    const data = params.data as IAppData;

    // see styled components server-side rendering - https://www.styled-components.com/docs/advanced#server-side-rendering
    const sheet = new ServerStyleSheet();

    const user : IUserContext = {id: "test", email: "test@example.com", role: "admin"};
    const Context = React.createContext(user);

    // static router = a router that never changes location (on server-side render things won't change)
    const app = (
      <Context.Provider value={user}>
        <StaticRouter context={routerContext} location={params.location.path}>
          <App compiler="Typescript" framework="React" />
        </StaticRouter>
      </Context.Provider>
    );

    // If there's a redirection, just send this information back to the host application
    if (routerContext.url) {
      resolve({ redirectUrl: routerContext.url });
      return;
    }

    const html = renderToString(sheet.collectStyles(app));

    // need to get this into the document head for styled components
    const styleTags = sheet.getStyleTags();

    // once any async tasks are done, we can perform the final render
    params.domainTasks.then(() => {
      resolve({
        html: getTemplate(html, styleTags, data) // you can also use this to initialize globals from the server-side
      });
    }, reject); // also propagate any errors back into the host application
  });
});
