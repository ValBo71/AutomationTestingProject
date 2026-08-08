/** Selectors for challenges about waiting, appearing and disappearing elements. */
export const DynamicSelectors = {
  // /dynamic_loading/1 and /2
  startButton: '#start button',
  loadingIndicator: '#loading',
  finishText: '#finish',

  // /dynamic_controls
  checkboxForm: '#checkbox-example',
  checkboxContainer: '#checkbox',
  /**
   * Deliberately scoped to the form rather than to #checkbox: the page ships
   * the checkbox as <div id="checkbox"><input></div>, but after a remove/add
   * cycle it comes back as <div><input id="checkbox"></div> - the id moves
   * from the wrapper onto the input. A #checkbox-scoped locator silently
   * stops matching at that point.
   */
  checkbox: '#checkbox-example input[type="checkbox"]',
  removeAddButton: '#checkbox-example button',
  inputForm: '#input-example',
  textInput: '#input-example input[type="text"]',
  enableDisableButton: '#input-example button',
  message: '#message',

  // /dynamic_content
  contentRows: '#content .row',
  contentImages: '#content .row img',
  contentText: '#content .row .large-10',

  // /disappearing_elements
  navLinks: 'ul li a',

  // /shifting_content/menu_element
  shiftingMenu: '#content ul li',

  // /infinite_scroll
  scrollBlocks: '.jscroll-added',

  // /slow
  slowHeading: 'h1',
} as const;

/** Selectors for DOM-structure challenges. */
export const DomSelectors = {
  // /challenging_dom
  challengeButtons: '.button',
  challengeTable: 'table',
  challengeTableRows: 'table tbody tr',
  challengeTableHeaders: 'table thead th',
  canvas: '#canvas',

  // /large
  noSiblingsParent: '.parent',
  deepestNoSiblings: '#no-siblings',
  siblingTable: '#large-table',
  siblingCell: '#sibling-2\\.3',

  // /tables
  table1: '#table1',
  table2: '#table2',
  tableRows: 'tbody tr',
  tableHeaders: 'thead th',

  // /shadowdom
  shadowHost: 'my-paragraph',

  // /broken_images
  images: '#content img',

  // /typos
  typoParagraph: '#content .example',
} as const;
