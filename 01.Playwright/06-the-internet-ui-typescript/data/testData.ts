/**
 * Central test data: every challenge path plus the strings the suite asserts on.
 * Keeping the routes here means a site restructure is a one-file change.
 */
export const Routes = {
  home: '/',
  abTest: '/abtest',
  addRemoveElements: '/add_remove_elements/',
  basicAuth: '/basic_auth',
  brokenImages: '/broken_images',
  challengingDom: '/challenging_dom',
  checkboxes: '/checkboxes',
  contextMenu: '/context_menu',
  digestAuth: '/digest_auth',
  disappearingElements: '/disappearing_elements',
  dragAndDrop: '/drag_and_drop',
  dropdown: '/dropdown',
  dynamicContent: '/dynamic_content',
  dynamicControls: '/dynamic_controls',
  dynamicLoadingHidden: '/dynamic_loading/1',
  dynamicLoadingRendered: '/dynamic_loading/2',
  entryAd: '/entry_ad',
  exitIntent: '/exit_intent',
  fileDownload: '/download',
  fileUpload: '/upload',
  floatingMenu: '/floating_menu',
  forgotPassword: '/forgot_password',
  formAuthentication: '/login',
  frames: '/iframe',
  geolocation: '/geolocation',
  horizontalSlider: '/horizontal_slider',
  hovers: '/hovers',
  infiniteScroll: '/infinite_scroll',
  inputs: '/inputs',
  jqueryUiMenu: '/jqueryui/menu',
  javaScriptAlerts: '/javascript_alerts',
  javaScriptOnloadError: '/javascript_error',
  keyPresses: '/key_presses',
  largeAndDeepDom: '/large',
  multipleWindows: '/windows',
  nestedFrames: '/nested_frames',
  /**
   * Visiting /notification_message sets a flash and redirects to the
   * _rendered page. Loading _rendered directly shows an empty banner, so the
   * test has to enter through this route.
   */
  notificationMessage: '/notification_message',
  notificationMessageRendered: '/notification_message_rendered',
  redirectLink: '/redirector',
  secureFileDownload: '/download_secure',
  shadowDom: '/shadowdom',
  shiftingContent: '/shifting_content/menu',
  slowResources: '/slow',
  sortableDataTables: '/tables',
  statusCodes: '/status_codes',
  typos: '/typos',
  wysiwygEditor: '/tinymce',
} as const;

export const Credentials = {
  /** Same pair works for both the Basic and the Digest auth challenges. */
  basicAuth: { username: 'admin', password: 'admin' },
  formAuth: { username: 'tomsmith', password: 'SuperSecretPassword!' },
  invalidUser: { username: 'wrong_user', password: 'wrong_password' },
} as const;

export const Expected = {
  // Which variation is served depends on a cookie; "No A/B Test" is what the
  // opt-out cookie produces. All three are legitimate outcomes.
  abTestHeadings: ['A/B Test Control', 'A/B Test Variation 1', 'No A/B Test'],
  basicAuthSuccess: 'Congratulations! You must have the proper credentials.',
  digestAuthSuccess: 'Congratulations! You must have the proper credentials.',
  loginSuccessFlash: 'You logged into a secure area!',
  loginInvalidUserFlash: 'Your username is invalid!',
  loginInvalidPasswordFlash: 'Your password is invalid!',
  logoutFlash: 'You logged out of the secure area!',
  forgotPasswordConfirmation: "Your e-mail's been sent!",
  uploadSuccessHeading: 'File Uploaded!',
  dynamicLoadingText: 'Hello World!',
  dynamicControlsGone: "It's gone!",
  dynamicControlsBack: "It's back!",
  dynamicControlsEnabled: "It's enabled!",
  contextMenuAlert: 'You selected a context menu',
  jsAlertText: 'I am a JS Alert',
  jsConfirmText: 'I am a JS Confirm',
  jsPromptText: 'I am a JS prompt',
  jsAlertResult: 'You successfully clicked an alert',
  jsConfirmOkResult: 'You clicked: Ok',
  jsConfirmCancelResult: 'You clicked: Cancel',
  shadowDomTexts: ["Let's have some different text!", 'In a list!'],
  entryAdModalTitle: 'This is a modal window',
  notificationMessages: [
    'Action successful',
    'Action unsuccesful, please try again',
  ],
  typoSentenceCorrect: "Sometimes you'll see a typo, other times you won't.",
  typoSentenceWrong: "Sometimes you'll see a typo, other times you won,t.",
  frameEditorDefaultText: 'Your content goes here.',
  nestedFrameTexts: {
    left: 'LEFT',
    middle: 'MIDDLE',
    right: 'RIGHT',
    bottom: 'BOTTOM',
  },
  newWindowHeading: 'New Window',
} as const;

export const TestData = {
  /** Value typed into the number field on /inputs. */
  numberInput: 42,
  /** Key sent to /key_presses; the page echoes back "You entered: <NAME>". */
  keyToPress: { key: 'A', expectedLabel: 'You entered: A' },
  /** Slider on /horizontal_slider accepts 0 to 5 in 0.5 steps. */
  sliderTarget: '3.5',
  /** Number of elements the add/remove spec creates before deleting them. */
  elementsToAdd: 5,
  /** Contents written into the file used by the upload spec. */
  uploadFileName: 'playwright-upload.txt',
  uploadFileContent: 'Uploaded by the Playwright suite.',
  /** Status codes the /status_codes challenge exposes. */
  statusCodes: [200, 301, 404, 500],
} as const;
