import React, { Component } from 'react';
import { View } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';

interface EventDialogProps {
}

export class EventDialogImpl extends Component<EventDialogProps> {
    public render() {
        const iconName = '';
        const iconSize = 0;

        const dialogTitle = '';
        const dialogText = '';

        return (
            <View>
                <View>
                    <ApplicationIcon name={iconName} size={iconSize} />
                </View>
                <View>
                    <StyledText>{dialogTitle}</StyledText>
                    <View>
                        <StyledText>{dialogText}</StyledText>
                    </View>

                    <View>
                        {/* Button 1 */}
                        {/* Button 2 */}
                    </View>
                </View>
                <View>
                    {/* Close dialog button */}
                </View>
            </View>
        );
    }
}

const mapStateToProps = (state: AppState) => ({
});

export const EventDialog = connect(mapStateToProps)(EventDialogImpl);