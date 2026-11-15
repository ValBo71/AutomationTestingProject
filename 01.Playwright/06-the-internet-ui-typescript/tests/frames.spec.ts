import { test, expect } from '@playwright/test';
import { FramePage } from '../pages/FramePage';
import { Expected } from '../data/testData';

test.describe('Frames', () => {
  let framePage: FramePage;

  test.beforeEach(async ({ page }) => {
    framePage = new FramePage(page);
  });

  test('WYSIWYG Editor: the editor loads inside its iframe and its content is readable', async () => {
    await framePage.openWysiwygEditor();

    // Crossing the iframe boundary is the actual skill this challenge tests,
    // and that part works even when the editor is read-only (see below).
    const editorBody = framePage.editorBody();
    await expect(editorBody).toBeVisible();
    await expect(editorBody).toContainText(Expected.frameEditorDefaultText);
  });

  /**
   * THIRD-PARTY QUOTA: the site loads TinyMCE from its cloud, on a monthly
   * quota of editor loads. When the quota is used up the editor renders with
   * contenteditable="false" and shows "TinyMCE is in read-only mode because
   * you have no more editor loads available this month" - typing is then
   * impossible for anyone (seen 2026-08 and 2026-09; editable again since
   * 2026-10).
   *
   * The state belongs to the site owner and flips without notice, so the test
   * skips with that reason instead of pinning either state.
   */
  test('WYSIWYG Editor: text can be typed inside the TinyMCE iframe', async () => {
    await framePage.openWysiwygEditor();

    const editorBody = framePage.editorBody();
    await expect(editorBody).toBeVisible();
    test.skip(
      (await editorBody.getAttribute('contenteditable')) === 'false',
      'TinyMCE is in read-only mode: the site has used up its monthly editor loads'
    );

    await framePage.typeInEditor('Automated by Playwright');

    await expect(editorBody).toContainText('Automated by Playwright');
  });

  test('Frames: the iframe example exposes the same editor', async () => {
    await framePage.openFramesExample();

    await expect(framePage.editorBody()).toContainText(Expected.frameEditorDefaultText);
  });

  test('Nested Frames: all four inner frames are reachable', async () => {
    await framePage.openNestedFrames();

    // frame-left/middle/right live inside frame-top, yet a single lookup by
    // name reaches them: page.frame() searches every frame in the page, nested
    // ones included, so no chaining through frame-top is needed.
    expect(await framePage.getNestedFrameTextAsync('frame-left')).toBe(
      Expected.nestedFrameTexts.left
    );
    expect(await framePage.getNestedFrameTextAsync('frame-middle')).toBe(
      Expected.nestedFrameTexts.middle
    );
    expect(await framePage.getNestedFrameTextAsync('frame-right')).toBe(
      Expected.nestedFrameTexts.right
    );
    expect(await framePage.getNestedFrameTextAsync('frame-bottom')).toBe(
      Expected.nestedFrameTexts.bottom
    );
  });
});
