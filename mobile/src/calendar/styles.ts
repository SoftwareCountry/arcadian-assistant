import { StyleSheet } from 'react-native';

const daysCounterFontColor = '#18515E';

export const calendarScreenColors = {
    green: '#27AE60',
    blue: '#56CCF2',
    return: '#EB5757',
    yellow: '#F2C94C'
};

export const calendarScreenLayout = {
    daysCounters: {
        flex: 1.2
    },
    calendar: {
        flex: 4
    },
    agenda: {
        flex: 3
    }
};

export const styles = StyleSheet.create({
    daysCounters: {
        flex: calendarScreenLayout.daysCounters.flex,
        flexDirection: 'row',
        padding: 10
    },
    daysCounterSeparator: {
        marginRight: 1
    },
    daysCounter: {
        backgroundColor: 'rgba(47, 175, 204, 0.33)',
        flexDirection: 'column',
        flexGrow: 1
    },
    daysCounterContent: {
        flexDirection: 'column',
        alignItems: 'center',
        flex: 20
    },
    daysCounterContentValue: {
        fontSize: 42,
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