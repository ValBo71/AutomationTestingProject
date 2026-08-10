/**
 * Every route the suite touches, mapped in one place.
 *
 * The platform is a set of separate services behind one gateway, which is why
 * the paths do not follow a single convention - each service was written on its
 * own terms. That inconsistency is part of what this suite documents.
 */
export const Api = {
  auth: {
    login: '/api/auth/login',
    validate: '/api/auth/validate',
    logout: '/api/auth/logout',
  },
  room: {
    base: '/api/room',
    byId: (id: number) => `/api/room/${id}`,
  },
  booking: {
    base: '/api/booking',
    byId: (id: number) => `/api/booking/${id}`,
    /** The collection endpoint refuses to answer without a room filter. */
    byRoom: (roomId: number) => `/api/booking?roomid=${roomId}`,
  },
  message: {
    base: '/api/message',
    byId: (id: number) => `/api/message/${id}`,
    /** Returns the number of *unread* messages, despite the name. */
    unreadCount: '/api/message/count',
    read: (id: number) => `/api/message/${id}/read`,
  },
  branding: '/api/branding',
  report: '/api/report',
} as const;

export const Ui = {
  home: '/',
  admin: '/admin',
  adminMessages: '/admin/message',
  adminReport: '/admin/report',
  adminBranding: '/admin/branding',
  reservation: (roomId: number) => `/reservation/${roomId}`,
} as const;
