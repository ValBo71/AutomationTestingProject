/** Selectors for frames, windows, downloads and navigation-flavoured challenges. */
export const FrameSelectors = {
  // /iframe (TinyMCE editor lives inside an iframe)
  editorIframe: '#mce_0_ifr',
  editorBody: '#tinymce',
  editorToolbarFile: 'button:has-text("File")',

  // /nested_frames
  topFrameName: 'frame-top',
  bottomFrameName: 'frame-bottom',
  leftFrameName: 'frame-left',
  middleFrameName: 'frame-middle',
  rightFrameName: 'frame-right',
  frameBody: 'body',
} as const;

export const NavigationSelectors = {
  // /windows
  newWindowLink: 'a[href="/windows/new"]',

  // /redirector
  redirectLink: '#redirect',

  // /status_codes
  statusCodeLinks: '#content a',
  statusCodeResult: '#content p',

  // /download
  downloadLinks: '#content a[href^="download/"]',

  // /upload
  fileInput: '#file-upload',
  uploadSubmit: '#file-submit',
  uploadedFiles: '#uploaded-files',
  dragDropArea: '#drag-drop-upload',

  // /geolocation
  whereAmIButton: 'button[onclick="getLocation()"]',
  latitude: '#lat-value',
  longitude: '#long-value',
  demoParagraph: '#demo',
} as const;
