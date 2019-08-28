import * as React from "react";
import { Header } from "./Header";
import { useScrollYPosition } from "react-use-scroll-position";

export interface ILayoutProps {
  drawerClickHandler: () => void;
}

// export function Layout(props: ILayoutProps) {
//   React.useEffect(() => {
//     function
//   });

//   return (
//     <div>
//       <Header mobileNavToggleHandler={this.mobileNavToggleHandler} />
//       <main style={contentScrollOffset} className={mobileNavContentClass}>
//         {this.props.children}
//       </main>
//     </div>
//   );
// }

interface ILayoutContainer {
  test: string;
}

const resetScroll = (isMobileNavOpen: boolean, contentScrollPosition: number) => {
  if(isMobileNavOpen) {
    window.scrollTo(0, 0);
  } else {
    // global.console.log("RESET SCROLL CORRECTLY: ", contentScrollPosition);
    window.scrollTo(0, contentScrollPosition);
  }
}

function LayoutContainer(props: React.PropsWithChildren<ILayoutContainer>) {
  // const layoutRef = React.useRef<HTMLElement>(null);
  const [isMobileNavOpen, setIsMobileNavOpen] = React.useState(false);
  const [contentScrollPosition, setContentScrollPosition] = React.useState(0);
  const onMobileToggle = React.useCallback((e: boolean) => { 
    global.console.log("Mobile Nave Callback", e);
    setIsMobileNavOpen(e); 
  }, []);
  
  const scrollY = useScrollYPosition();
  const contentScrollOffset = isMobileNavOpen ? { top: 70 - contentScrollPosition } : {};
  const mobileNavContentClass = isMobileNavOpen ? "mobileNavOpened" : "";
  React.useEffect(() => {
    // global.console.log("Last Scroll: ", scrollY);
    return () => {
      if(!isMobileNavOpen) {
        // global.console.log("SOMETHING CHANGED?????!!!!!?", scrollY);
        setContentScrollPosition(scrollY);
      }
    };
  }, [scrollY]);

  React.useEffect(() => {
    return () => {
      global.console.log("Is Mobile Nav Open: ", isMobileNavOpen);
      // global.console.log("Content Scroll Position: ", contentScrollPosition);
      // shouldn't need to invert isMobileNavOpen but for some reason it always take the previous state at this point
      resetScroll(!isMobileNavOpen, contentScrollPosition);
    }
  }, [isMobileNavOpen]);

  // global.console.log("Is Mobile Nav Open: ", isMobileNavOpen);
  // global.console.log("Is Mobile Nav Open: ", contentScrollPosition);
  return (
    <div>
      <Header mobileNavToggleHandler={onMobileToggle} />
      <main style={contentScrollOffset} className={mobileNavContentClass}>
        {props.children}
      </main>
    </div>
  );
}

export class Layout extends React.Component<ILayoutProps> {
  // private layoutRef : React.RefObject<HTMLElement> = React.useRef<HTMLElement>(null);
  constructor(props: any) {
    super(props);
  }

  // private mobileNavToggleHandler = (isMobileNavOpen: boolean) => {
  //   this.setState({ isMobileNavOpen });
  // };

  // public state: ILayoutState = {
  //   isMobileNavOpen: false,
  //   scrollPosition: 0
  // };

  // private scrollListener = (scrollY: number) => {
  //   // record the previous scroll state
  //   if (!this.state.isMobileNavOpen) {
  //     global.console.log("SCROLL Y FROM WITHIN: ", scrollY);
  //     // this.setState({
  //     //   scrollPosition: scrollY
  //     // });
  //   }
  // };

  public render() {
    return (
      <LayoutContainer test="string">
        {this.props.children}
      </LayoutContainer>
    );
  }
}
