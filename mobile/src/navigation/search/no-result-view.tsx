import React, { Component } from 'react';
import { StyledText } from '../../override/styled-text';
import { View } from 'react-native';
import { noResult } from './search-view-styles';

//============================================================================
export class NoResultView extends Component {

    //----------------------------------------------------------------------------
    public render() {
        return <View style={noResult.container}>
            <StyledText style={noResult.text}>{'No result'}</StyledText>
        </View>;
    }
}
