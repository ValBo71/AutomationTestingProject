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
    // and that part works regardless of the editor being read-only.
    const editorBody = framePage.editorBody();
    await expect(editorBody).toBeVisible();
    await expect(editorBody).toContainText(Expected.frameEditorDefaultText);
  });

  /**
   * KNOWN BLOCKER (third-party, verified 2026-08): the editor renders with
   * class "mce-content-readonly" and contenteditable="false", and TinyMCE
   * shows "TinyMCE is in read-only mode because you have no more editor loads
   * available this month". The site owner's TinyMCE cloud quota is exhausted,
   * so typing is impossible for anyone - not a locator or timing problem.
   *
   * Kept as an expected failure so the intended coverage stays visible and
   * the suite reports it the moment the quota resets.
   */
  test.fail('WYSIWYG Editor: text can be typed inside the TinyMCE iframe', async () => {
    await framePage.openWysiwygEditor();

    await framePage.typeInEditor('Automated by Playwright');

    await expect(framePage.editorBody()).toContainText('Automated by Playwright');
  });

  test('WYSIWYG Editor: the read-only state is reported by the editor itself', async () => {
    await framePage.openWysiwygEditor();

    // Pins the current third-party state, so the suite notices if it changes.
    await expect(framePage.editorBody()).toHaveAttribute('contenteditable', 'false');
  });

  test('Frames: the iframe example exposes the same editor', async () => {
    await framePage.openFramesExample();

    await expect(framePage.editorBody()).toContainText(Expected.frameEditorDefaultText);
  });

  test('Nested Frames: all four inner frames are reachable', async () => {
    await framePage.openNestedFrames();

    // frame-left/middle/right live inside frame-top, so reaching them means
    // resolving a frame within a frame rather than a single lookup.
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
