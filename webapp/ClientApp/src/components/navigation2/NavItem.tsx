import * as React from "react";

interface INavItemProps {
  navId: string;
  navTitle: string;
  selectedNavId: string | null;
  selectNavItem: (navId: string, hasChildren: boolean) => void;
}

export function NavItem(props: React.PropsWithChildren<INavItemProps>) {
  const subNavItems = React.Children.map(props.children, child => {
    return <li>{child}</li>;
  });

  const navClickHandler = () => {
    props.selectNavItem(props.navId, false);
  };

  const subNavClickHandler = () => {
    props.selectNavItem(props.navId, true);
  };

  const isSubNavShown =
    props.navId === props.selectedNavId
      ? { display: "block" }
      : { display: "none" };

  // Note react requires unique key to be set with array of child elements (e.g. see below)
  const navItem =
    subNavItems.length > 1
      ? [
          <a onClick={subNavClickHandler} key="1">
            {props.navTitle}
          </a>,
          props.children && (
            <ul className="nav-dropdown" style={isSubNavShown} key="2">
              {subNavItems}
            </ul>
          )
        ]
      : props.children;

  return <li onClick={navClickHandler}>{navItem}</li>;
}