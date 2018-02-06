import { StyleSheet, ViewStyle } from 'react-native';

const daysCounterFontColor = '#fff';
const daysCounterTitleColor = '#18515E';
const daysCounterPrimaryColor = '#2FAFCC';

const triangleHeight = 50;
const circleDiameter = 100;

export const calendarScreenLayout = {
    calendar: {
        flex: 1
    },
    agenda: {
        flex: 1
    }
};

export const calendarStyles = StyleSheet.create({
    container: {
        flex: calendarScreenLayout.calendar.flex,
        marginTop: 20
    },
    containerTitle: {
        fontSize: 15,
        color: daysCounterTitleColor
    }
});

export const agendaStyles = StyleSheet.create({
    container: {
        flex: calendarScreenLayout.calendar.flex,
        alignSelf: 'stretch'
    }
});

export const daysCountersStyles = StyleSheet.create({
    container: {
        flex: 1
    },
    counters: {
        flex: 1,
        flexDirection: 'row'
    }
});

export const daysCounterSeparatorStyles = StyleSheet.create({
    container: {
        width: circleDiameter,
        backgroundColor: daysCounterPrimaryColor
    }
});

export const daysCounterStyles = StyleSheet.create({
    container: {
        backgroundColor: daysCounterPrimaryColor,
        flexDirection: 'column',
        width: '50%',
        flex: 1
    },
    content: {
        marginTop: triangleHeight,
        flexDirection: 'column',
        alignItems: 'center',
        flex: 20
    },
    contentValue: {
        fontSize: 25,
        color: daysCounterFontColor
    },
    contentTitle: {
        fontSize: 11,
        color: daysCounterFontColor
    }
});

const selectedDayShapeZIndex = 2;

export const daysCounterSelectedDayStyles = StyleSheet.create({
    container: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: selectedDayShapeZIndex + 1,
        left: '50%',
        backgroundColor: '#fff',
        position: 'absolute',
        transform: [{ translateX: -50 }],
        alignItems: 'center',
        justifyContent: 'center',
        borderWidth: 2,
        borderColor: daysCounterPrimaryColor,
    },
    circleCurrentDay: {
        fontSize: 40,
        color: daysCounterTitleColor,
        marginTop: -5
    },
    circleCurrentMonth: {
        fontSize: 15,
        color: daysCounterTitleColor,
        marginTop: -8
    },
    triangle: {
        backgroundColor: 'transparent',
        width: 0,
        height: 0,
        zIndex: selectedDayShapeZIndex,
        borderBottomWidth: triangleHeight,
        borderStyle: 'solid',
        borderLeftColor: 'transparent',
        borderRightColor: 'transparent',
        borderBottomColor: '#fff',
        position: 'absolute',
        transform: [
            { rotate: '180deg' }
        ]
    }
});