/** Selectors for mouse/keyboard-heavy challenges. */
export const InteractionSelectors = {
  // /drag_and_drop
  columnA: '#column-a',
  columnB: '#column-b',
  columnHeader: 'header',

  // /context_menu
  hotSpot: '#hot-spot',

  // /hovers
  figures: '.figure',
  figureCaption: '.figcaption',
  figureCaptionHeading: 'h5',
  figureCaptionLink: 'a',

  // /jqueryui/menu
  menuRoot: '#ui-id-1',
  enabledMenuItem: '#ui-id-3',
  downloadsMenuItem: '#ui-id-4',
  pdfDownload: '#ui-id-5',
  csvDownload: '#ui-id-6',
  excelDownload: '#ui-id-7',
  backToJQueryUi: '#ui-id-2',

  // /floating_menu
  floatingMenu: '#menu',
  floatingMenuLinks: '#menu a',
} as const;

/** Selectors for the alert/modal/notification challenges. */
export const NotificationSelectors = {
  // /javascript_alerts
  alertButton: 'button[onclick="jsAlert()"]',
  confirmButton: 'button[onclick="jsConfirm()"]',
  promptButton: 'button[onclick="jsPrompt()"]',
  result: '#result',

  // /entry_ad
  modal: '#modal',
  // /exit_intent uses the ouibounce library, which renders its own wrapper id
  exitIntentModal: '#ouibounce-modal',
  modalTitle: '.modal-title h3',
  modalBody: '.modal-body',
  modalClose: '.modal-footer p',
  restartAd: '#restart-ad',

  // /notification_message_rendered
  flash: '#flash',
  newMessageLink: 'a[href="/notification_message"]',
} as const;
