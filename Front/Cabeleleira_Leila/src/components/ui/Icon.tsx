type IconName =
  | 'arrow-right'
  | 'bar-chart'
  | 'calendar'
  | 'chevron-left'
  | 'chevron-right'
  | 'clock'
  | 'currency'
  | 'edit'
  | 'log-in'
  | 'log-out'
  | 'scissors'
  | 'shield'
  | 'sparkle'
  | 'trash'
  | 'users'

interface IconProps {
  name: IconName
  size?: number
  decorative?: boolean
}

const paths: Record<IconName, string[]> = {
  'arrow-right': ['M5 12h14', 'M13 5l7 7-7 7'],
  'bar-chart': ['M4 19V9', 'M10 19V5', 'M16 19v-7', 'M3 19h18'],
  calendar: ['M8 2v4', 'M16 2v4', 'M3 10h18', 'M5 4h14a2 2 0 0 1 2 2v13a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2'],
  'chevron-left': ['M15 18l-6-6 6-6'],
  'chevron-right': ['M9 18l6-6-6-6'],
  clock: ['M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20', 'M12 6v6l4 2'],
  currency: ['M12 2v20', 'M17 7.5c-.7-1.4-2.2-2.5-5-2.5-3 0-5 1.5-5 3.7 0 5.3 10 2.3 10 7.6 0 2.2-2 3.7-5 3.7-2.8 0-4.4-1.1-5.1-2.6'],
  edit: ['M12 20h9', 'M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4 12.5-12.5z'],
  'log-in': ['M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4', 'M10 17l5-5-5-5', 'M15 12H3'],
  'log-out': ['M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4', 'M16 17l5-5-5-5', 'M21 12H9'],
  scissors: ['M14 14l7 7', 'M14 10l7-7', 'M5 9a3 3 0 1 0 0-6 3 3 0 0 0 0 6', 'M5 21a3 3 0 1 0 0-6 3 3 0 0 0 0 6', 'M8 8l6 6', 'M8 16l6-6'],
  shield: ['M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10'],
  sparkle: ['M12 2l2.4 6.6L21 11l-6.6 2.4L12 20l-2.4-6.6L3 11l6.6-2.4L12 2z'],
  trash: ['M3 6h18', 'M8 6V4h8v2', 'M6 6l1 15h10l1-15'],
  users: ['M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2', 'M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8', 'M22 21v-2a4 4 0 0 0-3-3.8', 'M16 3.1a4 4 0 0 1 0 7.8'],
}

const Icon = ({ name, size = 18, decorative = true }: IconProps) => (
  <svg
    aria-hidden={decorative}
    className="icon"
    fill="none"
    height={size}
    role={decorative ? undefined : 'img'}
    stroke="currentColor"
    strokeLinecap="round"
    strokeLinejoin="round"
    strokeWidth="2"
    viewBox="0 0 24 24"
    width={size}
  >
    {paths[name].map((path) => (
      <path d={path} key={path} />
    ))}
  </svg>
)

export default Icon
