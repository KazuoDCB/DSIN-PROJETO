import { buildMonthDays, monthTitle, sameDay } from '../../utils/calendar'
import Icon from '../ui/Icon'

interface CalendarPanelProps {
  datesWithAppointments: Date[]
  month: Date
  selectedDate: Date
  onMonthChange: (date: Date) => void
  onSelectDate: (date: Date) => void
}

const weekDays = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab']

const CalendarPanel = ({
  datesWithAppointments,
  month,
  selectedDate,
  onMonthChange,
  onSelectDate,
}: CalendarPanelProps) => {
  const days = buildMonthDays(month)

  const changeMonth = (offset: number) => {
    onMonthChange(new Date(month.getFullYear(), month.getMonth() + offset, 1))
  }

  return (
    <section className="calendar-panel">
      <header className="calendar-panel__header">
        <button aria-label="Mes anterior" className="icon-button" onClick={() => changeMonth(-1)} type="button">
          <Icon name="chevron-left" />
        </button>
        <h2>{monthTitle(month)}</h2>
        <button aria-label="Proximo mes" className="icon-button" onClick={() => changeMonth(1)} type="button">
          <Icon name="chevron-right" />
        </button>
      </header>

      <div className="calendar-panel__weekdays">
        {weekDays.map((day) => (
          <span key={day}>{day}</span>
        ))}
      </div>

      <div className="calendar-panel__days">
        {days.map((day) => {
          const hasAppointments = datesWithAppointments.some((date) => sameDay(date, day.date))
          const isSelected = sameDay(day.date, selectedDate)
          const classes = [
            'calendar-day',
            day.isCurrentMonth ? '' : 'calendar-day--muted',
            day.isToday ? 'calendar-day--today' : '',
            isSelected ? 'calendar-day--selected' : '',
          ]
            .filter(Boolean)
            .join(' ')

          return (
            <button className={classes} key={day.date.toISOString()} onClick={() => onSelectDate(day.date)} type="button">
              {day.day}
              {hasAppointments ? <span aria-label="Possui agendamento" /> : null}
            </button>
          )
        })}
      </div>
    </section>
  )
}

export default CalendarPanel
