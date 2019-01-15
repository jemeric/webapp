﻿import * as React from 'react';
import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import { renderToString } from 'react-dom/server';
import { StaticRouter } from 'react-router-dom';
import { App } from './components/App';
import { ServerStyleSheet } from 'styled-components';

export default createServerRenderer(params => {
    return new Promise<RenderResult>((resolve, reject) => {
        // this context object contains the results of the render (i.e. context.url will contain URL to redirect to if <Redirect> was used)
        const routerContext: any = {};

        const urls = params.data as string[];

        // see styled components server-side rendering - https://www.styled-components.com/docs/advanced#server-side-rendering
        const sheet = new ServerStyleSheet();

        // static router = a router that never changes location (on server-side render things won't change)
        var scriptUrls = urls.concat(",");
        const app = (
            <div id="example">
                <StaticRouter context={routerContext} location={params.location.path}>
                    <App compiler="Typescript" framework="React" />
                </StaticRouter>
            </div>
        );

        // If there's a redirection, just send this information back to the host application
        if (routerContext.url) {
            resolve({ redirectUrl: routerContext.url });
            return;
        }

        sheet.collectStyles(app);

        const html = renderToString(app);

        // TODO - we need to get this into the document head
        const styleTags = sheet.getStyleTags();

        // once any async tasks are done, we can perform the final render
        params.domainTasks.then(() => {
            resolve({
                html: styleTags + html + scriptUrls // you can also use this to initialize globals from the server-side
            });
        }, reject); // also propagate any errors back into the host application
    });
});