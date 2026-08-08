import { FrameLocator, Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { FrameSelectors } from '../selectors/NavigationSelectors';
import { Routes } from '../data/testData';

/** Covers Frames (WYSIWYG editor in an iframe) and Nested Frames. */
export class FramePage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /tinymce and /iframe -----

  async openWysiwygEditor() {
    await this.goto(Routes.wysiwygEditor);
  }

  async openFramesExample() {
    await this.goto(Routes.frames);
  }

  editorFrame(): FrameLocator {
    return this.page.frameLocator(FrameSelectors.editorIframe);
  }

  editorBody() {
    return this.editorFrame().locator(FrameSelectors.editorBody);
  }

  async typeInEditor(text: string) {
    const body = this.editorBody();
    await body.click();
    // The editor keeps its default paragraph, so clear it before typing.
    await this.page.keyboard.press('ControlOrMeta+a');
    await this.page.keyboard.type(text);
  }

  // ----- /nested_frames -----

  async openNestedFrames() {
    await this.goto(Routes.nestedFrames);
  }

  /**
   * The layout is a frameset: frame-top holds left/middle/right, and
   * frame-bottom sits beside it. Reaching the inner three means chaining
   * two frame lookups rather than one.
   */
  async getNestedFrameTextAsync(frameName: string): Promise<string> {
    const frame = this.page.frame({ name: frameName });
    if (!frame) {
      throw new Error(`Frame "${frameName}" was not found on the page.`);
    }
    return (await frame.locator(FrameSelectors.frameBody).innerText()).trim();
  }
}
