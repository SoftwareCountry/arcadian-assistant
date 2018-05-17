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
    eventIcon: {
        flex: 1,
        color: '#2FAFCC', 
        fontSize: 20, 
        marginRight: 10
    },
    eventTitle: {
        flex: 5,
        fontSize: 12 
    }
});
