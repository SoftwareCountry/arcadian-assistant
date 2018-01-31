import { StyleSheet } from 'react-native';

const daysCounterFontColor = '#18515E';
export const todayTitleColor = '#18515E';

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
        color: todayTitleColor
    }
});

export const agendaStyles = StyleSheet.create({
    container: {
        flex: calendarScreenLayout.calendar.flex,
        alignItems: 'flex-start'
    }
});

export const styles = StyleSheet.create({
    daysCounters: {
        flex: 1,
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