import { Dimensions, StyleSheet } from 'react-native';
import Style from '../layout/style';

const circleDiameter = Dimensions.get('window').width * 0.5;
const chevronHeight = 50;
const placeholderHeight = circleDiameter * 0.5;

export const layoutStylesForEmployeeDetailsScreen = StyleSheet.create({
    chevronPlaceholder: {
        height: Math.round(placeholderHeight - chevronHeight * 0.5),
        backgroundColor: Style.color.base
    },
    eventsContainer: {
        marginTop: 10,
        marginBottom: 10,
    },
    eventRow: {
        flex: 1,
        flexDirection: 'row',
        marginLeft: 20,
        marginRight: 20,
        alignItems: 'center'
    },
    eventLeftIcons: {
        height: 48,
        width: 86,
        marginRight: 8,
        flexDirection: 'row',
        alignSelf: 'center',
        justifyContent: 'space-between'
    },
    eventTypeIconContainer: {
        backgroundColor: '#D5EFF5',
        position: 'absolute',
        top: 1,
        left: 40,
        width: 46,
        height: 46,
        borderRadius: 23,
        justifyContent: 'center',
        alignItems: 'center'
    },
    eventLeftIconsTiny: {
        height: 48,
        width: 48,
        marginRight: 8,
        flexDirection: 'row',
        alignSelf: 'center',
        justifyContent: 'space-between'
    },
    eventTypeIconContainerTiny: {
        backgroundColor: '#D5EFF5',
        position: 'absolute',
        top: 1,
        left: 1,
        width: 46,
        height: 46,
        borderRadius: 23,
        justifyContent: 'center',
        alignItems: 'center'
    },
    eventIcon: {
        color: '#333',
        fontSize: 16
    },
    eventTextContainer: {
        flex: 5
    },
    eventTitle: {
        fontSize: 12
    },
    eventDetails: {
        fontSize: 10
    },
    avatarContainer: {
        position: 'absolute',
        top: 0,
        left: 0,
        width: 48,
        height: 48,
        alignSelf: 'center'
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 1
    }
});

export const employeeDetailsStyles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: 'transparent'
    },
    topHalfView: {
        position: 'absolute',
        backgroundColor: Style.color.base,
        top: 0,
        left: 0,
    },
    bottomHalfView: {
        position: 'absolute',
        backgroundColor: Style.color.white,
        left: 0,
    },
});

export const employeeDetailsTileStyles = StyleSheet.create({
    container: {
        width: '25%'
    },
    tile: {
        backgroundColor: 'rgba(47, 175, 204, 0.2)',
        paddingBottom: 8,
        paddingTop: 10,
        flexDirection: 'column',
        height: 66
    },
    iconContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
    },
    text: {
        fontSize: 11,
        textAlign: 'center',
        color: '#18515E',
        paddingTop: 2
    },
    separator: {
        flexBasis: 1,
        backgroundColor: Style.color.white
    }
});

export const employeeDetailsContactStyles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        height: 60,
        alignItems: 'center'
    },
    iconContainer: {
        width: 50,
        height: 40,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
    },
    textContainer: {
        paddingLeft: 20,
        flexDirection: 'column'
    },
    text: {
        fontSize: 14,
        lineHeight: 17,
        color: '#333'
    },
    title: {
        paddingBottom: 3,
        fontSize: 10,
        lineHeight: 12,
        color: 'rgba(0, 0, 0, 0.7)'
    }
});

export const employeeDetailsDaysCounterStyles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        height: 60,
        alignItems: 'center'
    },
    iconContainer: {
        width: 50,
        height: 40,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
    },
    textContainer: {
        paddingLeft: 20,
        flexDirection: 'column'
    },
    text: {
        fontSize: 14,
        lineHeight: 17,
        color: '#333'
    },
    title: {
        paddingBottom: 3,
        fontSize: 10,
        lineHeight: 12,
        color: 'rgba(0, 0, 0, 0.7)'
    }
});

export const layoutStylesForEventManagementToolset = StyleSheet.create({
    toolsetContainer: {
        marginLeft: 20,
        width: 72,
        flexDirection: 'row',
        justifyContent: 'space-between'
    },
    approveIcon: {
        color: '#3DB1CA',
        fontSize: 28
    },
    rejectIcon: {
        color: '#E7585D',
        fontSize: 28
    }
});
