import * as React from "react";
import { NavItem } from "./NavItem";
import { NavLink } from "react-router-dom";
import { useClickOutside } from "use-events";


// TODO - suggest as PR to make it safe for server-side rendering
function getWindowDimensions() : [ number, number ] {
  return isWindowDefined() ? [ window.innerWidth, window.innerHeight ] : [ 0, 0 ];
}

function isWindowDefined() : boolean {
  return typeof window !== 'undefined';
}

const useWindowResize = (): [number, number] => {
  const [winWidth, winHeight] = getWindowDimensions();
  const [width, setWidth] = React.useState(winWidth);
  const [height, setHeight] = React.useState(winHeight);

  React.useEffect(() => {
    const resize = () => {
      const [newWinWidth, newWinHeight] = getWindowDimensions();
      setWidth(newWinWidth);
      setHeight(newWinHeight);
    };

    if(isWindowDefined()) window.addEventListener('resize', resize);

    return () => {
      if(isWindowDefined()) window.removeEventListener('resize', resize);
    };
  }, []);

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

  useClickOutside([navRef], event => {
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    global.console.log("CLICKED OUTSIDE:", event); // TODO - this only seems to be picked up on the second click when non-mobile nav menu is open
    if (props.shouldCloseNavItem(event) && !isMobile(screenWidth)) {
      setSelectedNavId(null);
    }
  });

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

// export class NavMenu extends React.Component<INavMenuProps, INavMenuState> {
//   private navRef: HTMLElement | null = null;
//   constructor(props: any) {
//     super(props);
//     this.selectNavItem = this.selectNavItem.bind(this);
//     this.setNavRef = this.setNavRef.bind(this);
//     this.handleClickOutside = this.handleClickOutside.bind(this);
//     this.updateWindowDimensions = this.updateWindowDimensions.bind(this);
//   }

//   public componentDidMount() {
//     // because this is loaded isomorphically we must wait until the client has loaded to check this
//     // rather than in setting the initial state
//     this.updateWindowDimensions();
//     window.addEventListener("resize", this.updateWindowDimensions);
//     document.addEventListener("mousedown", this.handleClickOutside);
//   }

//   public componentWillUnmount() {
//     window.removeEventListener("resize", this.updateWindowDimensions);
//     document.removeEventListener("mousedown", this.handleClickOutside);
//   }

//   private isMobile(): boolean {
//     return this.state.screenWidth <= maxResponsiveWidth;
//   }

//   private updateWindowDimensions() {
//     // TODO - setup global react hook for this?
//     this.setState({ screenWidth: window.innerWidth });
//   }

//   private setNavRef = (node: HTMLElement) => {
//     this.navRef = node;
//   };

//   private handleClickOutside = (event: MouseEvent) => {
//     // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
//     if (
//       this.navRef &&
//       !this.navRef.contains(event.target as any) &&
//       this.props.shouldCloseNavItem(event) &&
//       !this.isMobile()
//     ) {
//       this.setState({ selectedNavId: null });
//     }
//   };

//   public state: INavMenuState = {
//     screenWidth: 0,
//     selectedNavId: null
//   };

//   private selectNavItem = (navId: string, hasChildren: boolean) => {
//     // don't collapse nav when clicked on if in mobile view (but collapse sub-nav if parent clicked)
//     if (!hasChildren && this.isMobile()) return;
//     if (this.state.selectedNavId === navId) {
//       // unset the navigation if null
//       this.setState({ selectedNavId: null });
//     } else {
//       this.setState({ selectedNavId: navId });
//     }
//   };

//   public render() {
//     const showMobileNav = this.props.isMobileNavOpen
//       ? { display: "block" }
//       : { display: "none" };
//     global.console.log(
//       "Show Mobile Nav: ",
//       showMobileNav,
//       this.props.isMobileNavOpen
//     );
//     return (
//       <nav ref={this.setNavRef}>
//         <ul className="nav-list" style={showMobileNav}>
//           <NavItem
//             navId="home"
//             navTitle="Home"
//             selectedNavId={this.state.selectedNavId}
//             selectNavItem={this.selectNavItem}
//           >
//             <a href="#!">Home</a>
//           </NavItem>
//           <NavItem
//             navId="about"
//             navTitle="About"
//             selectedNavId={this.state.selectedNavId}
//             selectNavItem={this.selectNavItem}
//           >
//             <a href="#!">About</a>
//           </NavItem>
//           <NavItem
//             navId="services"
//             navTitle="Services"
//             selectedNavId={this.state.selectedNavId}
//             selectNavItem={this.selectNavItem}
//           >
//             <a href="#!">Web Design</a>
//             <a href="#!">Web Developer</a>
//           </NavItem>
//           <NavItem
//             navId="portfolio"
//             navTitle="Portfolio"
//             selectedNavId={this.state.selectedNavId}
//             selectNavItem={this.selectNavItem}
//           >
//             <a href="#!">Web Design</a>
//             <a href="#!">Web Developer</a>
//           </NavItem>
//           <NavItem
//             navId="home"
//             navTitle="Home"
//             selectedNavId={this.state.selectedNavId}
//             selectNavItem={this.selectNavItem}
//           >
//             <a href="#!">Test</a>
//           </NavItem>
//         </ul>
//       </nav>
//     );
//   }
// }
