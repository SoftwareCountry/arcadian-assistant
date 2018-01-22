import { StyleSheet } from 'react-native';

const daysCounterHeight = 90;
const daysCounterIndicatorHeight = 4;
const daysCounterFontColor = '#18515E';

export const colors = {
    days: {
        left: '#27AE60',
        all: '#56CCF2',
        return:  '#EB5757',
        sick: '#F2C94C'
    }
};

export const styles = StyleSheet.create({
    container: {
        flex: 1,
        flexDirection: 'row',
        padding: 10
    },
    daysCounterSeparator: {
        marginRight: 1
    },
    daysCounter: {
        backgroundColor: 'rgba(47, 175, 204, 0.33)',
        height: daysCounterHeight,
        flexDirection: 'column',
        flexGrow: 1
    },
    daysCounterContent: {
        paddingTop: 6,
        paddingBottom: 15,
        paddingLeft: 10,
        paddingRight: 10,
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: daysCounterHeight - daysCounterIndicatorHeight
    },
    daysCounterLeftDays: {
        fontSize: 50,
        color: daysCounterFontColor
    },
    daysCounterAllDays: {
        fontSize: 20,
        color: daysCounterFontColor
    },
    daysCounterTitle: {
        fontSize: 12,
        color: daysCounterFontColor
    },
    daysCounterIndicator: {
        flexDirection: 'row',
        height: daysCounterIndicatorHeight
    }
});