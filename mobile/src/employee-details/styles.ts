import { StyleSheet, Dimensions } from 'react-native';

export const chevronColor = '#2FAFCC';

const circleDiameter = Dimensions.get('window').width * 0.5;
const chevronHeight = 50;
const placeholderHeight = circleDiameter * 0.5;

export const layoutStylesForEmployeeDetailsScreen = StyleSheet.create({
    chevronPlaceholder: {
        height: placeholderHeight - chevronHeight * 0.5,
        backgroundColor: chevronColor
    },
    eventsContainer: {
        marginTop: 5, 
        marginBottom: 5
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
    eventIcon: {
        color: '#333', 
        fontSize: 16
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
