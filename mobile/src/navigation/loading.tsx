import React from 'react';
import { ActivityIndicator, View } from 'react-native';
import { searchViewStyles as styles } from './search-view-styles';

//============================================================================
export class LoadingView extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator size={'large'} color={'#2FAFCC'}/>
            </View>
        );
    }
}
