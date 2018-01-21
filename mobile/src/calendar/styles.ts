import { StyleSheet } from 'react-native';

const tileHeight = 90;
const tileIndicatorHeight = 4;
const tileFontColor = '#18515E';

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
    tileSeparator: {
        marginRight: 1
    },
    tile: {
        backgroundColor: 'rgba(47, 175, 204, 0.33)',
        height: tileHeight,
        flexDirection: 'column',
        flexGrow: 1
    },
    tileContent: {
        paddingTop: 6,
        paddingBottom: 15,
        paddingLeft: 10,
        paddingRight: 10,
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: tileHeight - tileIndicatorHeight
    },
    tileLeftDays: {
        fontSize: 50,
        color: tileFontColor
    },
    tileAllDays: {
        fontSize: 20,
        color: tileFontColor
    },
    tileTitle: {
        fontSize: 12,
        color: tileFontColor
    },
    tileIndicator: {
        flexDirection: 'row',
        height: tileIndicatorHeight
    }
});