import * as React from "react";
import { NavLink } from "react-router-dom";
import DrawerToggleButton from "../SideDrawer/DrawToggleButton";

interface IToolbarProps {
  drawerClickHandler: () => void;
}

// navigation starter - https://www.academind.com/learn/react/snippets/navbar-side-drawer/
const Toolbar = (props: IToolbarProps) => (
  <header className="toolbar">
    <nav className="toolbar__navigation">
      <div className="toolbar__toggle-button">
        <DrawerToggleButton click={props.drawerClickHandler} />
      </div>
      <div className="toolbar__logo">
        <NavLink to="/" exact>
          THE LOGO
        </NavLink>
      </div>
      <div className="spacer" />
      <div className="toolbar_navigation-items">
        <ul>
          <li>
            <NavLink to="/" exact>
              Home
            </NavLink>
          </li>
          <li>
            <NavLink to="/users" exact>
              Users
            </NavLink>
          </li>
        </ul>
      </div>
    </nav>
  </header>
);

export default Toolbar;
