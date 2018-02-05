import { StyleSheet, ViewStyle } from 'react-native';

const daysCounterFontColor = '#fff';
const daysCounterTitleColor = '#18515E';
const daysCounterPrimaryColor = '#2FAFCC';

export const calendarScreenColors = {
    green: '#27AE60',
    blue: '#56CCF2',
    red: '#EB5757',
    yellow: '#F2C94C'
};

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

const triangleHeight = 50;
const circleDiameter = 100;

export const styles = StyleSheet.create({
    daysCounters: {
        flex: 1,
        flexDirection: 'row'
    },
    daysCounterSeparator: {
        width: circleDiameter,
        backgroundColor: daysCounterPrimaryColor
    },
    daysCounter: {
        backgroundColor: daysCounterPrimaryColor,
        flexDirection: 'column',
        width: '50%',
        flex: 1
    },
    daysCounterContent: {
        marginTop: triangleHeight,
        flexDirection: 'column',
        alignItems: 'center',
        flex: 20
    },
    daysCounterContentValue: {
        fontSize: 25,
        color: daysCounterFontColor
    },
    daysCounterContentTitle: {
        fontSize: 11,
        color: daysCounterFontColor
    }
});

const todayShapeZIndex = 2;

export const daysCounterTodayStyles = StyleSheet.create({
    container: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: todayShapeZIndex + 1,
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
        zIndex: todayShapeZIndex,
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