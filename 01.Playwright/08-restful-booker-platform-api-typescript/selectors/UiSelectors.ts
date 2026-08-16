/**
 * Selectors read from the running application, not from its source.
 *
 * The admin screens are generous with ids, and several of them embed the value
 * they display - a room called 101 renders as #roomName101. Convenient, but it
 * means an id is not a stable handle across a rename, so those are built as
 * functions rather than stored as constants.
 */
export const FrontPage = {
  roomsSection: '#rooms',
  roomCard: '#rooms .room-card',
  cardTitle: '.card-title',
  contact: {
    name: '#name',
    email: '#email',
    phone: '#phone',
    subject: '#subject',
    description: '#description',
  },
} as const;

export const AdminLogin = {
  username: '#username',
  password: '#password',
} as const;

export const AdminRooms = {
  /** Each existing room row carries this, which makes counting rows reliable. */
  row: '[data-testid="roomlisting"]',
  rowById: (roomId: number) => `#room${roomId}`,
  /** The id embeds the displayed value - see the note at the top of this file. */
  nameCell: (roomName: string) => `#roomName${roomName}`,
  priceCell: (price: number) => `#roomPrice${price}`,
  createForm: {
    name: '#roomName',
    type: '#type',
    accessible: '#accessible',
    price: '#roomPrice',
    wifi: '#wifiCheckbox',
    tv: '#tvCheckbox',
    safe: '#safeCheckbox',
    submit: '#createRoom',
  },
} as const;

export const AdminMessages = {
  row: (index: number) => `[data-testid="message${index}"]`,
  anyRow: '[data-testid^="message"]',
  deleteButton: (index: number) => `[data-testid="DeleteMessage${index}"]`,
} as const;
