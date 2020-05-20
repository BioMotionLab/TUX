/**
 * Copyright (c) 2017-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

const React = require('react');

const CompLibrary = require('../../core/CompLibrary.js');

const MarkdownBlock = CompLibrary.MarkdownBlock; /* Used to read markdown */
const Container = CompLibrary.Container;
const GridBlock = CompLibrary.GridBlock;

class HomeSplash extends React.Component {
  render() {
    const {siteConfig, language = ''} = this.props;
    const {baseUrl, docsUrl} = siteConfig;
    const docsPart = `${docsUrl ? `${docsUrl}/` : ''}`;
    const langPart = `${language ? `${language}/` : ''}`;
    const docUrl = doc => `${baseUrl}${docsPart}${langPart}${doc}`;

    const SplashContainer = props => (
      <div className="homeContainer">
        <div className="homeSplashFade">
          <div className="wrapper homeWrapper">{props.children}</div>
        </div>
      </div>
    );

    const Logo = props => (
      <div className="projectLogo">
        <img src={props.img_src} alt="Project Logo" />
      </div>
    );

    const ProjectTitle = props => (
      <h2 className="projectTitle">
        {props.title}
        <small>{props.tagline}</small>
      </h2>
    );

    const PromoSection = props => (
      <div className="section promoSection">
        <div className="promoRow">
          <div className="pluginRowBlock">{props.children}</div>
        </div>
      </div>
    );

    const Button = props => (
      <div className="pluginWrapper buttonWrapper">
        <a className="button" href={props.href} target={props.target}>
          {props.children}
        </a>
      </div>
    );

    return (
      <SplashContainer>
        <Logo img_src={`${baseUrl}img/favicon.ico`} />
        <div className="inner">
          <ProjectTitle tagline={siteConfig.tagline} title={siteConfig.title} />
          <PromoSection>
            <Button href={docUrl('GettingStarted.html')}>Learn</Button>
            <Button href={docUrl('Installation.html')}>Install</Button>
          </PromoSection>
        </div>
      </SplashContainer>
    );
  }
}

class Index extends React.Component {
  render() {
    const {config: siteConfig, language = ''} = this.props;
    const {baseUrl} = siteConfig;

    const Block = props => (
      <Container
        padding={['bottom', 'top']}
        id={props.id}
        background={props.background}>
        <GridBlock
          align="center"
          contents={props.children}
          layout={props.layout}
        />
      </Container>
    );


      const Design = () => (
          <Block background="dark">
              {[
                  {
                      content:
                          'The experimental design is configured using a simple interface in the unity editor. Add variables, set up counterbalancing, repetitions, randomization, and more, with no code required.',
                      image: `${baseUrl}img/designinterface.png`,
                      imageAlign: 'right',
                      title: 'Design your experiment with no code',
                  },
              ]}
          </Block>
      );
    
    const Runner = () => (
      <Block background="light">
        {[
          {
            content:
              'Write short scripts to define how your Unity scene should be manipulated, record your measurements, and bmlTUX will take care of the rest.',
            image: `${baseUrl}img/scriptexample.png`,
            imageAlign: 'left',
            title: 'Run experiments with minimal code',
          },
        ]}
      </Block>
    );

    const Iterate = () => (
      <Block background="dark">
        {[
          {
            content:
              'Modifications and additions to experiments are simple and easy. You can swap between experimental design configurations with a drag-and-drop interface. This is particularly useful for pilot testing and training.',
            image: `${baseUrl}img/iterative.png`,
            imageAlign: 'right',
            title: 'Work iteratively',
          },
        ]}
      </Block>
    );


    const Features = () => (
      <Block layout="fourColumn">
        {[
          {
            content: 'Prototype 3D experiments quickly and iteratively',
            image: `${baseUrl}img/undraw_3d.svg`,
            imageAlign: 'top',
            title: 'Experiments in minutes',
          },
          {
            content: 'Easy integration with SteamVR',
            image: `${baseUrl}img/undraw_augmented_reality_heuy.svg`,
            imageAlign: 'top',
            title: 'VR Ready',
          },
          {
            content: 'Not a pro programmer? No Problem.',
            image: `${baseUrl}img/undraw_dev_focus.svg`,
            imageAlign: 'top',
            title: 'Minimal Coding',
          },
        ]}
      </Block>
    );

    const Showcase = () => {
      if ((siteConfig.users || []).length === 0) {
        return null;
      }

      const showcase = siteConfig.users
        .filter(user => user.pinned)
        .map(user => (
          <a href={user.infoLink} key={user.infoLink}>
            <img src={user.image} alt={user.caption} title={user.caption} />
          </a>
        ));

      const pageUrl = page => baseUrl + (language ? `${language}/` : '') + page;

      return (
        <div className="productShowcaseSection paddingBottom">
          <h2>Who is Using This?</h2>
          <p>This project is used by all these people</p>
          <div className="logos">{showcase}</div>
          <div className="more-users">
            <a className="button" href={pageUrl('users.html')}>
              More {siteConfig.title} Users
            </a>
          </div>
        </div>
      );
    };

    return (
      <div>
        <HomeSplash siteConfig={siteConfig} language={language} />
        <div className="mainContainer">
          <Features />
          <Design />
          <Runner />
          <Iterate />
        </div>
      </div>
    );
  }
}

module.exports = Index;
