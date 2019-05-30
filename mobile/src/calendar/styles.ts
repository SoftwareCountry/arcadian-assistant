import { PixelRatio, StyleSheet } from 'react-native';
import { CalendarEventStatus, CalendarEventType } from '../reducers/calendar/calendar-event.model';
import Style from '../layout/style';

const daysCounterTitleColor = '#18515E';

export const calendarScreenLayout = {
    daysCounters: {
        flex: 2
    },
    calendar: {
        flex: 8
    },
    agenda: {
        flex: 5
    }
};

const intervalZIndex = 1;
const weekDayCircleZIndex = intervalZIndex + 1;
const intervalBoundaryZIndex = weekDayCircleZIndex + 1;
const weekDayTouchableZIndex = intervalBoundaryZIndex + 1;

export const weekCalendarStyles = {
    paddingLeft: 1,
    paddingRight: 1
};

export const calendarStyles = StyleSheet.create({
    pagerContainer: {
        marginTop: 25,
        marginBottom: 10,
        flex: calendarScreenLayout.calendar.flex,
        alignSelf: 'stretch',
        position: 'relative'
    },
    swipeableList: {
        flex: 1,
        flexDirection: 'row',
        position: 'absolute'
    },
    today: {
        marginBottom: 10,
        flexDirection: 'row',
        justifyContent: 'center'
    },
    todayTitle: {
        fontSize: 16,
        color: daysCounterTitleColor
    },
    weekContainer: {
        paddingLeft: weekCalendarStyles.paddingLeft,
        paddingRight: weekCalendarStyles.paddingRight
    },
    weeksNames: {
        flex: 1,
        flexDirection: 'row'
    },
    weekName: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    weeks: {
        flex: 7,
        flexDirection: 'column'
    },
    week: {
        flex: 1,
        flexDirection: 'row'
    },
    weekDayContainer: {
        alignItems: 'center',
        justifyContent: 'center'
    },
    weekDay: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'center'
    },
    weekDayTouchable: {
        position: 'absolute',
        backgroundColor: 'transparent',
        top: 0,
        bottom: 0,
        right: 0,
        left: 0,
        zIndex: weekDayTouchableZIndex
    },
    weekDayCircle: {
        justifyContent: 'center',
        alignItems: 'center'
    },
    weekDayCircleContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'transparent',
        zIndex: weekDayCircleZIndex
    },
    weekDayNumber: {
        lineHeight: 14,
        color: 'rgba(0, 0, 0, 0.8678)'
    },
    weekDayName: {
        fontSize: 12,
        lineHeight: 14,
        color: 'rgba(0, 0, 0, 0.5433)',
    }
});

export class CalendarEventsColor {
    public static defaultColor = Style.color.black;
    public static vacation = '#2F80ED';
    public static sickLeave = '#F2C94C';
    public static dayoff = '#EB5757';
    public static workout = '#219653';

    private static noOpacity = '';
    private static processedOpacity = '80';

    public static getColor(type: CalendarEventType, status?: CalendarEventStatus) {
        let color: string = CalendarEventsColor.defaultColor;

        switch (type) {
            case CalendarEventType.Vacation:
                color = CalendarEventsColor.vacation;
                break;

            case CalendarEventType.Sickleave:
                color = CalendarEventsColor.sickLeave;
                break;

            case CalendarEventType.Dayoff:
                color = CalendarEventsColor.dayoff;
                break;

            case CalendarEventType.Workout:
                color = CalendarEventsColor.workout;
                break;

            default:
                color = CalendarEventsColor.defaultColor;
                break;
        }

        return color.concat(CalendarEventsColor.getOpacity(type, status));
    }

    private static getOpacity(type: CalendarEventType, status?: CalendarEventStatus) {
        if (!status) {
            return this.noOpacity;
        }

        if (type === CalendarEventType.Vacation &&
            status === CalendarEventStatus.Processed) {
            return this.processedOpacity;
        }

        return this.noOpacity;
    }
}

// serjKim: Depends on count of overlapped intervals?..
const intervalOpacity = .89;
export const intervalMargin = 0.2;

export const calendarIntervalStyles = StyleSheet.create({
    container: {
        position: 'absolute',
        width: '100%',
        alignItems: 'center',
        justifyContent: 'flex-end',
        flexDirection: 'row',
        opacity: intervalOpacity,
        zIndex: intervalZIndex
    },
    halfInterval: {
        width: '30%'
    },
    interval: {
        flex: 1
    },
    boundary: {
        zIndex: intervalBoundaryZIndex
    },
    selection: {
        opacity: 0.6
    }
});

export const agendaStyles = StyleSheet.create({
    container: {
        flex: calendarScreenLayout.agenda.flex,
        alignSelf: 'stretch'
    },
    controls: {
        flex: 1,
        alignSelf: 'stretch',
        flexDirection: 'row',
        paddingTop: 5,
        paddingBottom: 5
    },
    dialog: {
        flex: 1
    }
});

export const agendaSelectedDayStyles = StyleSheet.create({
    container: {
        flex: 3,
        borderRightWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.2)'
    }
});

export const calendarActionsStyles = StyleSheet.create({
    container: {
        flex: 4,
        flexDirection: 'column',
        alignContent: 'space-between',
        marginLeft: 20,
        marginRight: 20,
    },
    buttonContainer: {
        flex: 3
    },
    button: {
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 1,
        borderColor: CalendarEventsColor.vacation
    },
    buttonTitle: {
        fontSize: 12,
        lineHeight: 14,
        color: '#18515E'
    },
    separator: {
        flex: 1
    }
});

export const selectedDayStyles = StyleSheet.create({
    container: {
        alignItems: 'center',
        justifyContent: 'center'
    },
    circleCurrentDay: {
        fontSize: 42,
        lineHeight: 51,
        color: daysCounterTitleColor
    },
    circleCurrentMonth: {
        fontSize: 16,
        lineHeight: 20,
        color: daysCounterTitleColor
    }
});

const legendMarkerSize = 12;
export const legendStyles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        paddingTop: 15
    },
    itemContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingBottom: 5
    },
    marker: {
        width: legendMarkerSize,
        height: legendMarkerSize,
        borderRadius: PixelRatio.roundToNearestPixel(legendMarkerSize / 2)
    },
    label: {
        paddingLeft: 10,
        color: '#18515E',
        fontSize: 10,
        lineHeight: 12
    }
});
