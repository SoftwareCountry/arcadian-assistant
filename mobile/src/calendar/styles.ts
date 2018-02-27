import { StyleSheet, ViewStyle, PixelRatio } from 'react-native';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';

const daysCounterTitleColor = '#18515E';

export const calendarScreenLayout = {
    daysCounters: {
        flex: 2
    },
    calendar: {
        flex: 8
    },
    agenda: {
        flex: 4
    }
};

const weekDayElementsZIndex = 1;
export const calendarStyles = StyleSheet.create({
    container: {
        marginTop: 25,
        marginBottom: 10,
        marginLeft: 8,
        marginRight: 8,
        flex: calendarScreenLayout.calendar.flex,
        alignSelf: 'stretch'
    },
    today: {
        marginBottom: 10,
        flexDirection: 'row',
        justifyContent: 'space-between'
    },
    todayTitle: {
        fontSize: 16,
        color: daysCounterTitleColor
    },
    weeksContainer: {
        flex: 1
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
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    weekDay: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'center'
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
        zIndex: weekDayElementsZIndex + 1
    },
    weekDayNumber: {
        fontSize: 12,
        //fontFamily: 'Roboto' TODO: import font ?
    },
    weekDaySelectedNumber: {
        fontWeight: '500'
    },
    weekDayText: {
        fontSize: 12,
        color: 'rgba(0, 0, 0, 0.5433)',
        fontWeight: '500',
        //fontFamily: 'Roboto' TODO: import font ?
    }
});

export class CalendarEventsColor {
    public static vacation = '#2F80ED';
    public static sickLeave = '#F2C94C';
    public static dayoff = '#EB5757';
    public static additionalWork = '#18515E'; //TODO: replace with the right color

    public static getColor(type: CalendarEventsType) {
        switch (type) {
            case CalendarEventsType.Vacation:
                return CalendarEventsColor.vacation;

            case CalendarEventsType.SickLeave:
                return CalendarEventsColor.sickLeave;

            case CalendarEventsType.Dayoff:
                return CalendarEventsColor.dayoff;

            case CalendarEventsType.AdditionalWork:
                return CalendarEventsColor.additionalWork;

            default:
                return null;
        }
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
        zIndex: weekDayElementsZIndex
    },
    halfInterval: {
        width: '50%'
    },
    interval: {
        flex: 1
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
    button: {
        flex: 3,
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