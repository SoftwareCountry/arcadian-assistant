import { StyleSheet, ViewStyle } from 'react-native';

const daysCounterTitleColor = '#18515E';
const daysCounterPrimaryColor = '#2FAFCC';

const circleDiameter = 120;

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
        fontSize: 14,
        lineHeight: 17,
        color: daysCounterTitleColor
    },
    weeksContainer: {
        flex: 1
    },
    weeksNames: {
        flex: 1,
        flexDirection: 'row',
        //marginBottom: 10,
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
    weekDayText: {
        fontSize: 12,
        lineHeight: 14,
        color: 'rgba(0, 0, 0, 0.5433)'
    }
});

export const calendarIntervalColors = {
    vacation: '#2F80ED',
    sickLeave: '#F2C94C',
    dayoff: '#EB5757'
};

// serjKim: Depends on count of overlapped intervals?..
const intervalOpacity = .95;
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
        alignSelf: 'stretch',
        flexDirection: 'row',
        paddingTop: 5,
        paddingBottom: 5
    }
});

export const agendaTodayStyles = StyleSheet.create({
    container: {
        flex: 5,
        borderRightWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.2)'
    }
});

export const agendaButtonsStyles = StyleSheet.create({
    container: {
        flex: 8
    }
});

const daysCounterShapeZIndex = 2;

export const selectedDayStyles = StyleSheet.create({
    container: {
        //transform: [{ translateX: -(circleDiameter / 2) }],
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