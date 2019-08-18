import * as React from "react";
import { NavLink } from "react-router-dom";

interface ISideDrawerProps {
  show: boolean;
}

const SideDrawer = (props: ISideDrawerProps) => {
  let drawClasses = "side-drawer";
  if (props.show) {
    drawClasses = "side-drawer open";
  }
  return (
    <nav className={drawClasses}>
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
        <li>
            <a href="#">
              Account
            </a>
            <ul className="nav-dropdown">
              <li>
                <a href="/playground">
                  GraphQL
                </a>
              </li>
              <li>
                <a href="/playground">
                  Logout
                </a>
              </li>
            </ul>
          </li>
      </ul>
    </nav>
  );
};

export default SideDrawer;
