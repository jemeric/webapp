import * as React from "react";
import { NavItem } from "./NavItem";
import { NavLink } from "react-router-dom";
import { useClickOutside } from "use-events";

// TODO - see https://github.com/sandiiarov/use-events/pull/167 (update and remove this if it gets accepted)
function getWindowDimensions(
  serverSideWidth: number,
  serverSideHeight: number
): [number, number] {
  return isWindowDefined()
    ? [window.innerWidth, window.innerHeight]
    : [serverSideWidth, serverSideHeight];
}

function isWindowDefined(): boolean {
  return typeof window !== "undefined";
}

const useWindowResize = (
  serverSideWidth: number = 0,
  serverSideHeight: number = 0
): [number, number] => {
  const [winWidth, winHeight] = getWindowDimensions(
    serverSideWidth,
    serverSideHeight
  );
  const [width, setWidth] = React.useState(winWidth);
  const [height, setHeight] = React.useState(winHeight);

  React.useEffect(() => {
    const resize = () => {
      const [newWinWidth, newWinHeight] = getWindowDimensions(
        serverSideWidth,
        serverSideHeight
      );
      setWidth(newWinWidth);
      setHeight(newWinHeight);
    };

    if (isWindowDefined()) window.addEventListener("resize", resize);

    return () => {
      if (isWindowDefined()) window.removeEventListener("resize", resize);
    };
  }, [serverSideWidth, serverSideHeight]);

  return [width, height];
};

interface INavMenuProps {
  shouldCloseNavItem: (event: MouseEvent) => boolean;
  isMobileNavOpen: boolean;
}

interface INavMenuState {
  screenWidth: number;
  selectedNavId: string | null;
}

const maxResponsiveWidth = 798;

function isMobile(screenWidth: number): boolean {
  return screenWidth <= maxResponsiveWidth;
}

export function NavMenu(props: INavMenuProps) {
  const [screenWidth] = useWindowResize();
  const [selectedNavId, setSelectedNavId] = React.useState<string | null>(null);
  const navRef = React.useRef<HTMLElement>(null);

  // use a callback here otherwise it will be re-run on each render and consequently might remove the listener before it can be re-added
  const outerMouseClickHandler = React.useCallback(event => {
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    if (props.shouldCloseNavItem(event) && !isMobile(screenWidth)) {
      setSelectedNavId(null);
    }
  }, [navRef, screenWidth]);
  useClickOutside([navRef], outerMouseClickHandler);

  const selectNavItem = (navId: string, hasChildren: boolean) => {
    // don't collapse nav when clicked on if in mobile view (but collapse sub-nav if parent clicked)
    if (!hasChildren && isMobile(screenWidth)) return;
    if (selectedNavId === navId) {
      // unset the navigation if null
      setSelectedNavId(null);
    } else {
      setSelectedNavId(navId);
    }
  };

  const showMobileNav = props.isMobileNavOpen
    ? { display: "block" }
    : { display: "none" };
  return (
    <nav ref={navRef}>
      <ul className="nav-list" style={showMobileNav}>
        <NavItem
          navId="home"
          navTitle="Home"
          selectedNavId={selectedNavId}
          selectNavItem={selectNavItem}
        >
          <a href="#!">Home</a>
        </NavItem>
        <NavItem
          navId="about"
          navTitle="About"
          selectedNavId={selectedNavId}
          selectNavItem={selectNavItem}
        >
          <a href="#!">About</a>
        </NavItem>
        <NavItem
          navId="services"
          navTitle="Services"
          selectedNavId={selectedNavId}
          selectNavItem={selectNavItem}
        >
          <a href="#!">Web Design</a>
          <a href="#!">Web Developer</a>
        </NavItem>
        <NavItem
          navId="portfolio"
          navTitle="Portfolio"
          selectedNavId={selectedNavId}
          selectNavItem={selectNavItem}
        >
          <a href="#!">Web Design</a>
          <a href="#!">Web Developer</a>
        </NavItem>
        <NavItem
          navId="home"
          navTitle="Home"
          selectedNavId={selectedNavId}
          selectNavItem={selectNavItem}
        >
          <a href="#!">Test</a>
        </NavItem>
      </ul>
    </nav>
  );
}