/**
 * Central test data for the UI Test Automation Playground.
 * Every challenge is a deliberate trap, so the comments here record what each
 * page is actually testing rather than just naming the route.
 */
/**
 * The Geolocation and Clipboard APIs are exposed only in a secure context, so
 * the two specs that use them must load the site over https. The certificate
 * does not cover this host, which is why playwright.config.ts sets
 * ignoreHTTPSErrors - that flag lets the connection succeed while still giving
 * the page a secure origin.
 */
export const SecureBaseUrl = 'https://uitestingplayground.com';

export const Routes = {
  home: '/home',
  ajaxData: '/ajax',
  alerts: '/alerts',
  animatedButton: '/animation',
  autoWait: '/autowait',
  classAttribute: '/classattr',
  clearInput: '/clearinput',
  click: '/click',
  clientSideDelay: '/clientdelay',
  cssSelectors: '/cssselectors',
  disabledInput: '/disabledinput',
  dynamicId: '/dynamicid',
  dynamicTable: '/dynamictable',
  frames: '/frames',
  geoLocation: '/geolocation',
  hiddenLayers: '/hiddenlayers',
  loadDelay: '/loaddelay',
  mouseOver: '/mouseover',
  nonBreakingSpace: '/nbsp',
  overlappedElement: '/overlapped',
  progressBar: '/progressbar',
  sampleApp: '/sampleapp',
  scrollbars: '/scrollbars',
  scrollToClick: '/scrolltoclick',
  select: '/select',
  shadowDom: '/shadowdom',
  textInput: '/textinput',
  fileUpload: '/upload',
  verifyText: '/verifytext',
  visibility: '/visibility',
} as const;

/**
 * U+00A0. Built with fromCharCode rather than pasted as a character or an
 * escape, so neither a formatter nor a careless edit can silently turn it
 * back into an ordinary space - the very confusion the /nbsp page is about.
 */
const NBSP = String.fromCharCode(0x00a0);

export const Expected = {
  ajaxSuccessText: 'Data loaded with AJAX get request.',
  clientDelaySuccessText: 'Data calculated on the client side.',
  classAttributeAlert: 'Primary button pressed',
  clickSuccessClass: 'btn-success',
  autoWaitSuccess: 'Target clicked',
  animationSuccess: 'Target clicked',
  disabledInputSuccess: 'Enabled',
  hiddenLayersGreenLabel: 'Button',
  sampleAppLoggedIn: (user: string) => `Welcome, ${user}!`,
  sampleAppInvalid: 'Invalid username/password',
  /**
   * The caption is rendered with a non-breaking space (U+00A0). The escape
   * sequence is spelled out rather than pasted, because the two characters are
   * indistinguishable in a source file - which is exactly how this bug hides
   * in real code.
   */
  nbspButtonRaw: `My${NBSP}Button`,
  nbspButtonNormalised: 'My Button',
  /** The DOM text is padded with newlines the browser collapses on screen. */
  verifyTextNormalised: 'Welcome UserName!',
  /** Hovering the first link retitles it from "Click me" to "Active Link". */
  mouseOverLinkTitleBefore: 'Click me',
  mouseOverLinkTitleAfter: 'Active Link',
  scrollToClickComplete: 'All buttons clicked!',
  disabledInputDisabled: 'Input Disabled',
  disabledInputEnabled: 'Input Enabled',
  dynamicTableColumns: ['Name', 'Memory', 'CPU', 'Disk', 'Network'],
} as const;

export const TestData = {
  sampleApp: {
    /** Any non-empty user name is accepted; only the password is validated. */
    username: 'qa.tester',
    password: 'pwd',
    wrongPassword: 'not-the-password',
  },
  textInput: {
    newButtonName: 'Renamed by Playwright',
  },
  select: {
    language: { value: 'py', label: 'Python' },
    city: { value: 'sf' },
  },
  upload: {
    fileName: 'playground-upload.txt',
    fileContent: 'Uploaded by the Playwright suite.',
  },
  /** Coordinates pinned so the geolocation assertion is deterministic. */
  geolocation: { latitude: 42.6977, longitude: 23.3219 },
  /**
   * The AJAX and client-delay pages both stall for roughly 15 seconds by
   * design, so they need a wait well above the project default.
   */
  slowChallengeTimeout: 30_000,
} as const;
