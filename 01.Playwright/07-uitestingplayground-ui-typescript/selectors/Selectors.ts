/** Selectors for the locator-strategy challenges. */
export const LocatorSelectors = {
  // /dynamicid - the id is regenerated on every load, so it must not be used
  dynamicIdButton: 'button.btn-primary',

  /**
   * /classattr - every button carries "btn" and "btn-test" plus one colour
   * class, so the colour has to be combined with btn-test to stay unique.
   * An exact-match selector such as [class="btn-primary"] finds nothing.
   */
  classAttributeButton: 'button.btn-test.btn-primary',
  allClassButtons: 'button.btn-test',

  // /nbsp - the caption is separated by U+00A0, not by an ordinary space
  nbspButton: 'button.btn-primary',

  // /textinput
  newButtonNameInput: '#newButtonName',
  updatingButton: '#updatingButton',

  // /verifytext - text is padded with whitespace the browser collapses
  verifyTextBadge: '.bg-primary .badge-secondary',

  // /cssselectors
  primaryButton: '#primary-btn',
  highlightedClassButton: '.css-btn.highlight',
  // These two inputs render with no id at all, so the type attribute is the
  // only handle - the page's own markup lists a duplicate id that the browser
  // discards.
  usernameInput: '.attr-section input[type="text"]',
  emailInput: '.attr-section input[type="email"]',
  linkByHref: 'a[href="https://example.com"]',
  adjacentParagraph: '.first-para + .second-para',
  visibleButton: '#visible-btn',
  hiddenDisplayButton: '#hidden-display',
  hiddenVisibilityButton: '#hidden-visibility',
  hiddenOpacityButton: '#hidden-opacity',
  hiddenOffscreenButton: '#hidden-offscreen',
} as const;

/** Selectors for the timing / waiting challenges. */
export const WaitSelectors = {
  // /ajax and /clientdelay share the same trigger id
  triggerButton: '#ajaxButton',
  loadedLabel: '.bg-success',

  // /loaddelay - the button only exists once the page finishes loading
  delayedButton: 'button.btn-primary',

  // /autowait
  elementTypeSelect: '#element-type',
  visibleCheckbox: '#visible',
  enabledCheckbox: '#enabled',
  editableCheckbox: '#editable',
  onTopCheckbox: '#ontop',
  nonZeroCheckbox: '#nonzero',
  applyAfter3Seconds: '#applyButton3',
  applyAfter5Seconds: '#applyButton5',
  target: '#target',

  // /progressbar
  startButton: '#startButton',
  stopButton: '#stopButton',
  progressBar: '#progressBar',

  // /disabledinput
  disabledInputField: '#inputField',
  enableButton: '#enableButton',

  // /animation
  startAnimationButton: '#animationButton',
  movingTarget: '#movingTarget',
} as const;

/** Selectors for challenges where something blocks or hides the target. */
export const ObstructionSelectors = {
  // /click - ignores DOM-dispatched clicks, only a real mouse event registers
  badButton: '#badButton',

  // /hiddenlayers - a second layer is drawn over the green button after a click
  greenButton: '#greenButton',
  blueButton: '#blueButton',

  // /overlapped - the input is behind a fixed overlay inside a scrollable box
  overlappedIdInput: '#id',
  overlappedNameInput: '#name',
  overlappedSubjectInput: '#subject',

  // /scrollbars - the button sits far inside a scrollable container
  hidingButton: '#hidingButton',

  // /scrolltoclick - four targets, each needing a different scroll strategy
  scrollTarget1: '#scrollTarget1',
  scrollTarget2: '#scrollTarget2',
  scrollTarget3: '#scrollTarget3',
  scrollTarget4: '#scrollTarget4',
  hoverRow4: '#targetRow4',
  progressText: '#progressText',

  // /visibility - eight buttons, each hidden a different way
  hideButton: '#hideButton',
  removedButton: '#removedButton',
  zeroWidthButton: '#zeroWidthButton',
  overlappedButton: '#overlappedButton',
  transparentButton: '#transparentButton',
  invisibleButton: '#invisibleButton',
  notDisplayedButton: '#notdisplayedButton',
  offscreenButton: '#offscreenButton',
} as const;

/** Selectors for challenges that require switching context (frame or shadow root). */
export const ContextSelectors = {
  // /frames - identical markup in an outer frame and a frame nested inside it
  outerFrame: '#frame-outer',
  innerFrame: '#frame-inner',
  frameLabel: '.frame-label',
  editButtonByData: 'button[data-action="edit"]',
  submitButtonByText: 'button:text-is("Submit")',
  clickMeButtonByName: 'button[name="my-button"]',
  primaryButtonByClass: 'button.btn-class',
  frameResult: '#result',

  // /shadowdom - a custom element whose controls live in an open shadow root
  guidGenerator: 'guid-generator',
  guidEditField: '#editField',
  guidGenerateButton: '#buttonGenerate',
  guidCopyButton: '#buttonCopy',

  // /upload - a React drag-and-drop uploader inside an iframe
  uploadFrame: 'iframe',
  uploadFileInput: 'input[type="file"]',
  uploadDropZone: '.drag-drop',
} as const;

/** Selectors for the remaining interactive challenges. */
export const AppSelectors = {
  /**
   * /sampleapp - the two inputs are rendered with random GUID ids on every
   * load, so only the name attribute is stable. The login button keeps a
   * fixed id.
   */
  username: 'input[name="UserName"]',
  password: 'input[name="Password"]',
  loginButton: '#login',
  loginStatus: '#loginstatus',

  // /alerts
  alertButton: '#alertButton',
  confirmButton: '#confirmButton',
  promptButton: '#promptButton',

  // /select
  languageSelect: '#selectLanguage',
  languageStatus: '#statusLanguage',
  citySelect: '#selectCity',
  cityStatus: '#statusCity',

  // /clearinput
  clearTargets: '.clear-target',
  clearTextInput: '#clearInput',
  clearTextarea: '#clearTextarea',
  clearNumberInput: '#clearNumber',
  clearContentEditable: '#clearContentEditable',

  // /mouseover - the link is replaced in the DOM on hover (stale element trap)
  mouseOverLink: 'a.text-primary',
  clickCount: '#clickCount',
  clickButtonCount: '#clickButtonCount',

  // /dynamictable - an ARIA table whose column order changes per load
  ariaTable: '[role="table"]',
  columnHeader: '[role="columnheader"]',
  tableRow: '[role="row"]',
  tableCell: '[role="cell"]',
  chromeCpuLabel: '.bg-warning',

  // /geolocation
  requestLocationButton: '#requestLocation',
  locationOutput: '#location',
} as const;
