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

export const styles = StyleSheet.create({
    daysCounters: {
        flex: 1,
        flexDirection: 'row'
    },
    daysCounterSeparator: {
        marginRight: 1
    },
    daysCounter: {
        backgroundColor: '#2FAFCC',
        flexDirection: 'column',
        width: '50%',
        flex: 1,
        // TODO: temp
        borderWidth: 2,
        borderColor: '#000',
    },
    daysCounterTriangel: {

    },
    daysCounterContent: {
        marginTop: triangleHeight, // TODO: temp
        flexDirection: 'column',
        alignItems: 'center',
        flex: 20,
    },
    daysCounterContentValue: {
        fontSize: 18,
        color: daysCounterFontColor
    },
    daysCounterContentTitle: {
        fontSize: 12,
        color: daysCounterFontColor
    },
    daysCounterIndicator: {
        flex: 1
    }
});

const todayShapeZIndex = 2;

export const daysCounterTodayStyles = StyleSheet.create({
    container: {
        borderRadius: 100 / 2,
        height: 100,
        width: 100,
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
    circle: {
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: '#fff',
        color: daysCounterTitleColor,
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