import * as React from "react";
import { Header } from "./Header";
import { useScrollYPosition } from "react-use-scroll-position";
import styled from "styled-components";

const resetScroll = (
  isMobileNavOpen: boolean,
  contentScrollPosition: number
) => {
  if (isMobileNavOpen) {
    window.scrollTo(0, 0);
  } else {
    window.scrollTo(0, contentScrollPosition);
  }
};

interface ILayoutContentProps {
  contentTop: number;
}

const LayoutContent = styled("main")<ILayoutContentProps>`
  // Page content
  max-width: $content-width;
  margin: 0 auto;
  margin-top: 80px;
  @media only screen and (max-width: 798px) {
    margin-top: 0px;
    &.mobileNavOpened {
      top: ${props => props.contentTop}px;
      position: fixed;
      right: 400px;
      width: 100vh;
    }
  }
`;

export function Layout(props: React.PropsWithChildren<{}>) {
  // const layoutRef = React.useRef<HTMLElement>(null);
  const [isMobileNavOpen, setIsMobileNavOpen] = React.useState(false);
  const [contentScrollPosition, setContentScrollPosition] = React.useState(0);
  const onMobileToggle = (e: boolean) => {
    if (e !== isMobileNavOpen) setIsMobileNavOpen(e);
  };

  const scrollY = useScrollYPosition();
  const contentTop = 70 - contentScrollPosition;
  const mobileNavContentClass = isMobileNavOpen ? "mobileNavOpened" : "";
  React.useEffect(() => {
    if (!isMobileNavOpen) {
      // store the most recent scroll state when the mobile nav was closed
      setContentScrollPosition(scrollY);
    }
  }, [scrollY]);

  React.useEffect(() => {
    resetScroll(isMobileNavOpen, contentScrollPosition);
    // Note** if useEffect returns a function React will run it when it's time to cleanup
  }, [isMobileNavOpen]);

  return (
    <div>
      <Header mobileNavToggleHandler={onMobileToggle} />
      <LayoutContent contentTop={contentTop} className={mobileNavContentClass}>
        {props.children}
      </LayoutContent>
    </div>
  );
}
