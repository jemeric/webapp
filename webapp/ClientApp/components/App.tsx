import * as React from "react";
import { Route, Switch } from "react-router";
import { NavLink } from "react-router-dom";

export interface AppProps { compiler: string; framework: string; }

//export const Hello = (props: HelloProps) => <h1>Hello from {props.compiler} and {props.framework}!</h1>;


const HomePage = () => <div>Home Page</div>
const UsersPage = () => <div>Users Page</div>

export class App extends React.Component<AppProps, {}> {
    render() {
        return <div>
            <header>
                <h1>Hello from {this.props.compiler} and {this.props.framework}!!</h1>
                <NavLink to="/" exact>Home</NavLink> | <NavLink to="/users">Users</NavLink>
            </header>
            <main>
                {/* use switch for exclusive routing - just 1 match */}
                <Switch>
                    <Route path="/" exact component={HomePage} />
                    <Route path="/users" component={UsersPage} />
                </Switch>
            </main>
        </div>;

    }
}