/** Selectors for the form-oriented challenges: checkboxes, dropdown, inputs, add/remove. */
export const FormSelectors = {
  // /checkboxes
  checkboxForm: '#checkboxes',
  checkboxes: '#checkboxes input[type="checkbox"]',

  // /dropdown
  dropdown: '#dropdown',

  // /inputs
  numberInput: 'input[type="number"]',

  // /add_remove_elements/
  addElementButton: 'button[onclick="addElement()"]',
  addedElements: '.added-manually',
  elementsContainer: '#elements',

  // /horizontal_slider
  slider: 'input[type="range"]',
  sliderValue: '#range',

  // /key_presses
  keyPressTarget: '#target',
  keyPressResult: '#result',
} as const;

/** Selectors for the two authentication forms. */
export const AuthSelectors = {
  // /login
  username: '#username',
  password: '#password',
  submitButton: 'button[type="submit"]',
  logoutButton: 'a[href="/logout"]',
  secureAreaHeading: 'h2',

  // /forgot_password
  email: '#email',
  forgotPasswordSubmit: '#form_submit',
  confirmationHeading: 'h1',
} as const;
